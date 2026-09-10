using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Executions.Commands.CreateExecutionAssignment;

public sealed record CreateExecutionAssignmentCommand(
    long ExecutionId,
    long ProviderId,
    string Role,
    bool? IsPrimary = null) : IRequest<long>;

public sealed class CreateExecutionAssignmentCommandValidator : AbstractValidator<CreateExecutionAssignmentCommand>
{
    public CreateExecutionAssignmentCommandValidator()
    {
        RuleFor(x => x.ExecutionId).GreaterThan(0);
        RuleFor(x => x.ProviderId).GreaterThan(0);
        RuleFor(x => x.Role).NotEmpty().MaximumLength(100);
    }
}

public sealed class CreateExecutionAssignmentCommandHandler : IRequestHandler<CreateExecutionAssignmentCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceExecutionRepository _executions;
    private readonly IExecutionAssignmentRepository _assignments;

    public CreateExecutionAssignmentCommandHandler(
        ICurrentUserService currentUserService,
        IServiceExecutionRepository executions,
        IExecutionAssignmentRepository assignments)
    {
        _currentUserService = currentUserService;
        _executions = executions;
        _assignments = assignments;
    }

    public async Task<long> Handle(CreateExecutionAssignmentCommand command, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var execution = await _executions.GetTrackedForPartyAsync(
            command.ExecutionId,
            userId,
            cancellationToken);

        if (execution is null)
            throw new KeyNotFoundException("Execution was not found.");

        if (execution.Deal.Proposal.ProviderId is not null)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("executionId", "A provider-party execution cannot receive assignments.")
            });
        }

        var businessId = execution.Deal.Proposal.BusinessId;
        if (businessId is null || execution.Deal.Proposal.Business!.OwnerUserId != userId)
            throw new KeyNotFoundException("Execution was not found.");

        if (!string.Equals(execution.Status, "Pending", StringComparison.Ordinal))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("executionId", "Providers can only be assigned while the execution is pending.")
            });
        }

        if (!await _assignments.ProviderExistsAsync(command.ProviderId, cancellationToken)
            || !await _assignments.HasActiveMembershipAsync(businessId.Value, command.ProviderId, cancellationToken))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("providerId", "Provider is not an active member of this business.")
            });
        }

        var hasPrimary = await _assignments.HasPrimaryAsync(execution.Id, cancellationToken);
        var isPrimary = command.IsPrimary == true;
        if (isPrimary && hasPrimary)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("isPrimary", "A primary assignment already exists.")
            });
        }

        if (await _assignments.HasAssignedProviderAsync(execution.Id, command.ProviderId, cancellationToken))
            throw new ConflictException("This provider is already assigned to the execution.");

        if (command.IsPrimary != true && !hasPrimary)
            isPrimary = true;

        var assignment = ExecutionAssignment.Create(
            execution.Id,
            command.ProviderId,
            command.Role,
            isPrimary);

        _assignments.Add(assignment);
        await _assignments.SaveChangesAsync(cancellationToken);

        return assignment.Id;
    }
}

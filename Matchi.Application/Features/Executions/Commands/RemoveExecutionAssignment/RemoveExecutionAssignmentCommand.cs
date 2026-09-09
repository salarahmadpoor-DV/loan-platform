using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Executions.Commands.RemoveExecutionAssignment;

public sealed record RemoveExecutionAssignmentCommand(long ExecutionId, long AssignmentId) : IRequest<bool>;

public sealed class RemoveExecutionAssignmentCommandHandler : IRequestHandler<RemoveExecutionAssignmentCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IExecutionAssignmentRepository _assignments;

    public RemoveExecutionAssignmentCommandHandler(
        ICurrentUserService currentUserService,
        IExecutionAssignmentRepository assignments)
    {
        _currentUserService = currentUserService;
        _assignments = assignments;
    }

    public async Task<bool> Handle(RemoveExecutionAssignmentCommand command, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var assignment = await _assignments.GetTrackedForBusinessOwnerAsync(
            command.ExecutionId,
            command.AssignmentId,
            userId,
            cancellationToken);

        if (assignment is null)
            throw new KeyNotFoundException("Assignment was not found.");

        if (assignment.ServiceExecution.Deal.Proposal.ProviderId is not null)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("executionId", "A provider-party execution cannot receive assignments.")
            });
        }

        if (!string.Equals(assignment.ServiceExecution.Status, "Pending", StringComparison.Ordinal))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("executionId", "Assignments can only be removed while the execution is pending.")
            });
        }

        try
        {
            assignment.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure("assignmentId", ex.Message) });
        }

        await _assignments.SaveChangesAsync(cancellationToken);
        return true;
    }
}

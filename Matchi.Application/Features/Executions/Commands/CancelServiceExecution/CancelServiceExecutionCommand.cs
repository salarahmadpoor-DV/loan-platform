using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Executions.Commands.CancelServiceExecution;

public sealed record CancelServiceExecutionCommand(long ExecutionId) : IRequest<string>;

public sealed class CancelServiceExecutionCommandHandler : IRequestHandler<CancelServiceExecutionCommand, string>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceExecutionRepository _executions;

    public CancelServiceExecutionCommandHandler(
        ICurrentUserService currentUserService,
        IServiceExecutionRepository executions)
    {
        _currentUserService = currentUserService;
        _executions = executions;
    }

    public async Task<string> Handle(CancelServiceExecutionCommand command, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var execution = await _executions.GetTrackedWithAssignmentsForPartyAsync(
            command.ExecutionId,
            userId,
            cancellationToken);

        if (execution is null)
            throw new KeyNotFoundException("Execution was not found.");

        try
        {
            execution.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure("executionId", ex.Message) });
        }

        foreach (var assignment in execution.Assignments)
        {
            if (string.Equals(assignment.Status, "Assigned", StringComparison.Ordinal))
                assignment.Cancel();
        }

        await _executions.SaveChangesAsync(cancellationToken);
        return execution.Status;
    }
}

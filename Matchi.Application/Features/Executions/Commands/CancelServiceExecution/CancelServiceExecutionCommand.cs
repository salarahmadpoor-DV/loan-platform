using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Application.Notifications;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Executions.Commands.CancelServiceExecution;

public sealed record CancelServiceExecutionCommand(long ExecutionId) : IRequest<string>;

public sealed class CancelServiceExecutionCommandHandler : IRequestHandler<CancelServiceExecutionCommand, string>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceExecutionRepository _executions;
    private readonly INotificationService _notifications;
    private readonly NotificationRecipientResolver _recipients;

    public CancelServiceExecutionCommandHandler(
        ICurrentUserService currentUserService,
        IServiceExecutionRepository executions,
        INotificationService notifications,
        NotificationRecipientResolver recipients)
    {
        _currentUserService = currentUserService;
        _executions = executions;
        _notifications = notifications;
        _recipients = recipients;
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

        var parties = await _recipients.ExecutionPartyUserIdsAsync(execution, cancellationToken);
        await _notifications.NotifyManyAsync(
            parties,
            NotificationCatalog.ExecutionCancelled(execution.Id),
            cancellationToken,
            excludeUserId: userId);

        return execution.Status;
    }
}

using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Application.Notifications;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Executions.Commands.UpdateServiceExecutionSchedule;

public sealed record UpdateServiceExecutionScheduleCommand(
    long ExecutionId,
    DateOnly? ScheduledDate = null,
    TimeSpan? ScheduledTimeFrom = null,
    TimeSpan? ScheduledTimeTo = null) : IRequest<bool>;

public sealed class UpdateServiceExecutionScheduleCommandValidator
    : AbstractValidator<UpdateServiceExecutionScheduleCommand>
{
    public UpdateServiceExecutionScheduleCommandValidator()
    {
        RuleFor(x => x.ExecutionId).GreaterThan(0);
        RuleFor(x => x.ScheduledTimeFrom)
            .Must((cmd, from) => from is null || cmd.ScheduledTimeTo is null || from < cmd.ScheduledTimeTo)
            .WithMessage("ScheduledTimeFrom must be earlier than ScheduledTimeTo.");
    }
}

public sealed class UpdateServiceExecutionScheduleCommandHandler
    : IRequestHandler<UpdateServiceExecutionScheduleCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceExecutionRepository _executions;
    private readonly INotificationService _notifications;
    private readonly NotificationRecipientResolver _recipients;

    public UpdateServiceExecutionScheduleCommandHandler(
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

    public async Task<bool> Handle(
        UpdateServiceExecutionScheduleCommand command,
        CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var execution = await _executions.GetTrackedForPartyAsync(
            command.ExecutionId,
            userId,
            cancellationToken);

        if (execution is null)
            throw new KeyNotFoundException("Execution was not found.");

        try
        {
            execution.UpdateSchedule(
                command.ScheduledDate,
                command.ScheduledTimeFrom,
                command.ScheduledTimeTo);
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure("executionId", ex.Message) });
        }

        await _executions.SaveChangesAsync(cancellationToken);

        var parties = await _recipients.ExecutionPartyUserIdsAsync(execution, cancellationToken);
        await _notifications.NotifyManyAsync(
            parties,
            NotificationCatalog.ExecutionScheduled(execution.Id),
            cancellationToken,
            excludeUserId: userId);

        return true;
    }
}

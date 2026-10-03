using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Notifications.Commands;

public sealed record MarkNotificationAsReadCommand(long NotificationId) : IRequest<bool>;

public sealed class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
{
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notifications;

    public MarkNotificationAsReadCommandHandler(
        ICurrentUserService currentUser,
        INotificationRepository notifications)
    {
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task<bool> Handle(MarkNotificationAsReadCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Authenticated user was not found.");

        var notification = await _notifications.GetTrackedForUserAsync(
            command.NotificationId,
            userId,
            cancellationToken);

        if (notification is null)
            throw new KeyNotFoundException("Notification was not found.");

        if (notification.MarkRead())
            await _notifications.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public sealed record MarkAllNotificationsAsReadCommand : IRequest<int>;

public sealed class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, int>
{
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notifications;

    public MarkAllNotificationsAsReadCommandHandler(
        ICurrentUserService currentUser,
        INotificationRepository notifications)
    {
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task<int> Handle(MarkAllNotificationsAsReadCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Authenticated user was not found.");

        return await _notifications.MarkAllReadAsync(userId, cancellationToken);
    }
}

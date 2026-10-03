using Matchi.Application.Notifications;

namespace Matchi.Application.Common.Interfaces;

public interface INotificationService
{
    Task NotifyAsync(
        long userId,
        NotificationDefinition definition,
        CancellationToken cancellationToken = default);

    Task NotifyManyAsync(
        IEnumerable<long> userIds,
        NotificationDefinition definition,
        CancellationToken cancellationToken = default,
        long? excludeUserId = null);
}

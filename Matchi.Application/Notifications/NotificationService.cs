using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Notifications;

public sealed class NotificationService : INotificationService
{
    private readonly INotificationRepository _notifications;

    public NotificationService(INotificationRepository notifications)
    {
        _notifications = notifications;
    }

    public Task NotifyAsync(
        long userId,
        NotificationDefinition definition,
        CancellationToken cancellationToken = default)
    {
        return NotifyManyAsync([userId], definition, cancellationToken);
    }

    public async Task NotifyManyAsync(
        IEnumerable<long> userIds,
        NotificationDefinition definition,
        CancellationToken cancellationToken = default,
        long? excludeUserId = null)
    {
        var distinct = userIds
            .Where(id => id > 0 && id != excludeUserId)
            .Distinct()
            .ToList();

        var added = 0;
        foreach (var userId in distinct)
        {
            if (await _notifications.ExistsAsync(userId, definition.ReferenceKey, cancellationToken))
                continue;

            await _notifications.AddAsync(
                new Notification(
                    userId,
                    definition.Type,
                    definition.Title,
                    definition.Message,
                    definition.EntityType,
                    definition.EntityId,
                    definition.ActionUrl,
                    definition.ReferenceKey),
                cancellationToken);
            added++;
        }

        if (added > 0)
            await _notifications.SaveChangesAsync(cancellationToken);
    }
}

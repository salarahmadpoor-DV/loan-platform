using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(long userId, string referenceKey, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Notification> Items, int TotalCount, int UnreadCount)> ListForUserAsync(
        long userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountUnreadAsync(long userId, CancellationToken cancellationToken = default);

    Task<Notification?> GetTrackedForUserAsync(
        long notificationId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<int> MarkAllReadAsync(long userId, CancellationToken cancellationToken = default);

    Task<long?> GetCustomerUserIdAsync(long customerId, CancellationToken cancellationToken = default);

    Task<long?> GetProviderUserIdAsync(long providerId, CancellationToken cancellationToken = default);

    Task<long?> GetBusinessOwnerUserIdAsync(long businessId, CancellationToken cancellationToken = default);
}

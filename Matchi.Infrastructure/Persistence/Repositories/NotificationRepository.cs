using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
    private readonly MatchiDbContext _context;

    public NotificationRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _context.Notifications.Add(notification);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (
            SqlServerUpdateConflicts.IsUniqueIndex(ex, "UX_Notifications_UserId_ReferenceKey")
            && PendingChangesAreNotificationsOnly())
        {
            foreach (var entry in _context.ChangeTracker.Entries<Notification>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified)
                    entry.State = EntityState.Detached;
            }
        }
    }

    private bool PendingChangesAreNotificationsOnly()
    {
        return _context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .All(e => e.Entity is Notification);
    }

    public Task<bool> ExistsAsync(long userId, string referenceKey, CancellationToken cancellationToken = default)
    {
        return _context.Notifications.AnyAsync(
            n => n.UserId == userId && n.ReferenceKey == referenceKey,
            cancellationToken);
    }

    public async Task<(IReadOnlyList<Notification> Items, int TotalCount, int UnreadCount)> ListForUserAsync(
        long userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Notifications.AsNoTracking().Where(n => n.UserId == userId);
        var totalCount = await query.CountAsync(cancellationToken);
        var unreadCount = await query.CountAsync(n => !n.IsRead, cancellationToken);
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .ThenByDescending(n => n.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount, unreadCount);
    }

    public Task<int> CountUnreadAsync(long userId, CancellationToken cancellationToken = default)
    {
        return _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
    }

    public Task<Notification?> GetTrackedForUserAsync(
        long notificationId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Notifications.FirstOrDefaultAsync(
            n => n.Id == notificationId && n.UserId == userId,
            cancellationToken);
    }

    public async Task<int> MarkAllReadAsync(long userId, CancellationToken cancellationToken = default)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in unread)
            notification.MarkRead();

        if (unread.Count > 0)
            await _context.SaveChangesAsync(cancellationToken);

        return unread.Count;
    }

    public Task<long?> GetCustomerUserIdAsync(long customerId, CancellationToken cancellationToken = default)
    {
        return _context.Customers
            .AsNoTracking()
            .Where(c => c.Id == customerId && !c.IsDeleted)
            .Select(c => (long?)c.UserId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<long?> GetProviderUserIdAsync(long providerId, CancellationToken cancellationToken = default)
    {
        return _context.Providers
            .AsNoTracking()
            .Where(p => p.Id == providerId && !p.IsDeleted)
            .Select(p => (long?)p.UserId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<long?> GetBusinessOwnerUserIdAsync(long businessId, CancellationToken cancellationToken = default)
    {
        return _context.Businesses
            .AsNoTracking()
            .Where(b => b.Id == businessId && !b.IsDeleted)
            .Select(b => (long?)b.OwnerUserId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

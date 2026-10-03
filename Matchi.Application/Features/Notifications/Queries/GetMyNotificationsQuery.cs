using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Notifications.Queries;

public sealed record GetMyNotificationsQuery(int Page = 1, int PageSize = 20) : IRequest<NotificationListDto>;

public sealed class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, NotificationListDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notifications;

    public GetMyNotificationsQueryHandler(
        ICurrentUserService currentUser,
        INotificationRepository notifications)
    {
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task<NotificationListDto> Handle(GetMyNotificationsQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Authenticated user was not found.");

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 20 : query.PageSize;

        var (items, totalCount, unreadCount) = await _notifications.ListForUserAsync(
            userId,
            page,
            pageSize,
            cancellationToken);

        return new NotificationListDto(
            items.Select(n => new NotificationDto(
                n.Id,
                n.Type,
                n.Title,
                n.Message,
                n.EntityType,
                n.EntityId,
                n.ActionUrl,
                n.IsRead,
                n.CreatedAt,
                n.ReadAt)).ToList(),
            page,
            pageSize,
            totalCount,
            unreadCount);
    }
}

public sealed record GetUnreadNotificationCountQuery : IRequest<UnreadNotificationCountDto>;

public sealed class GetUnreadNotificationCountQueryHandler
    : IRequestHandler<GetUnreadNotificationCountQuery, UnreadNotificationCountDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _notifications;

    public GetUnreadNotificationCountQueryHandler(
        ICurrentUserService currentUser,
        INotificationRepository notifications)
    {
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task<UnreadNotificationCountDto> Handle(
        GetUnreadNotificationCountQuery query,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("Authenticated user was not found.");

        var count = await _notifications.CountUnreadAsync(userId, cancellationToken);
        return new UnreadNotificationCountDto(count);
    }
}

namespace Matchi.Application.Features.Notifications;

public sealed record NotificationDto(
    long Id,
    string Type,
    string Title,
    string Message,
    string? EntityType,
    long? EntityId,
    string? ActionUrl,
    bool IsRead,
    DateTime CreatedAt,
    DateTime? ReadAt);

public sealed record NotificationListDto(
    IReadOnlyList<NotificationDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int UnreadCount);

public sealed record UnreadNotificationCountDto(int UnreadCount);

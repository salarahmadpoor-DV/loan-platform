using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Notification : Entity
{
    public long UserId { get; private set; }

    public string Type { get; private set; } = null!;

    public string Title { get; private set; } = null!;

    public string Message { get; private set; } = null!;

    public string? EntityType { get; private set; }

    public long? EntityId { get; private set; }

    public string? ActionUrl { get; private set; }

    public string? ReferenceKey { get; private set; }

    public bool IsRead { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ReadAt { get; private set; }

    public User User { get; private set; } = null!;

    private Notification()
    {
    }

    public Notification(
        long userId,
        string type,
        string title,
        string message,
        string? entityType = null,
        long? entityId = null,
        string? actionUrl = null,
        string? referenceKey = null)
    {
        if (userId <= 0)
            throw new InvalidOperationException("UserId must be greater than zero.");
        if (string.IsNullOrWhiteSpace(type))
            throw new InvalidOperationException("Type is required.");
        if (string.IsNullOrWhiteSpace(title))
            throw new InvalidOperationException("Title is required.");
        if (string.IsNullOrWhiteSpace(message))
            throw new InvalidOperationException("Message is required.");

        UserId = userId;
        Type = type.Trim();
        Title = title.Trim();
        Message = message.Trim();
        EntityType = string.IsNullOrWhiteSpace(entityType) ? null : entityType.Trim();
        EntityId = entityId is <= 0 ? null : entityId;
        ActionUrl = string.IsNullOrWhiteSpace(actionUrl) ? null : actionUrl.Trim();
        ReferenceKey = string.IsNullOrWhiteSpace(referenceKey) ? null : referenceKey.Trim();
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    public bool MarkRead()
    {
        if (IsRead)
            return false;

        IsRead = true;
        ReadAt = DateTime.UtcNow;
        return true;
    }
}

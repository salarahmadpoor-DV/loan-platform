using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ConversationParticipant : Entity
{
    public long ConversationId { get; private set; }

    public long UserId { get; private set; }

    public DateTime JoinedAt { get; private set; }

    public DateTime? LeftAt { get; private set; }

    public Conversation Conversation { get; private set; } = null!;

    public User User { get; private set; } = null!;

    private ConversationParticipant()
    {
    }

    public ConversationParticipant(long conversationId, long userId)
    {
        ConversationId = conversationId;
        UserId = userId;
    }
}

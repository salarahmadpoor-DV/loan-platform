using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Message : Entity
{
    public long ConversationId { get; private set; };

    public long SenderUserId { get; private set; };

    public string Text { get; private set; } = null!;

    public bool IsRead { get; private set; } = 0;

    public DateTime CreatedAt { get; private set; };

    public Conversation Conversation { get; private set; } = null!;

    public User SenderUser { get; private set; } = null!;

    private Message()
    {
    }

    public Message(long conversationId, long senderUserId, string text)
    {
        ConversationId = conversationId;
        SenderUserId = senderUserId;
        Text = text;
    }
}

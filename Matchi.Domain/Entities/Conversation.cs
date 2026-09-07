using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Conversation : Entity
{
    public long RequestId { get; private set; }

    public long? BusinessId { get; private set; }

    public long CustomerId { get; private set; }

    public DateTime CreateDate { get; private set; }

    public Business? Business { get; private set; }

    public Customer Customer { get; private set; } = null!;

    public Request Request { get; private set; } = null!;

    public ICollection<ConversationParticipant> Participants { get; private set; } = new List<ConversationParticipant>();

    public ICollection<Message> Messages { get; private set; } = new List<Message>();

    private Conversation()
    {
    }

    public Conversation(long requestId, long customerId)
    {
        RequestId = requestId;
        CustomerId = customerId;
    }
}

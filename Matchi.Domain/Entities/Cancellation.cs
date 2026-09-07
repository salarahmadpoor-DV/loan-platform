using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Cancellation : Entity
{
    public long RequestId { get; private set; }

    public long? DealId { get; private set; }

    public long CancelledByUserId { get; private set; }

    public string Reason { get; private set; } = null!;

    public string? Description { get; private set; }

    public DateTime CreateDate { get; private set; }

    public Deal? Deal { get; private set; }

    public Request Request { get; private set; } = null!;

    public User CancelledByUser { get; private set; } = null!;

    private Cancellation()
    {
    }

    public Cancellation(long requestId, long cancelledByUserId, string reason)
    {
        RequestId = requestId;
        CancelledByUserId = cancelledByUserId;
        Reason = reason;
    }
}

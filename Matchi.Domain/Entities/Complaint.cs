using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Complaint : TimestampedEntity
{
    public long RequestId { get; private set; };

    public long? DealId { get; private set; };

    public long CustomerId { get; private set; };

    public long? BusinessId { get; private set; };

    public long? ProviderId { get; private set; };

    public string Type { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public string Status { get; private set; } = "Open";

    public string? Resolution { get; private set; };

    public DateTime? ResolvedAt { get; private set; };

    public Business? Business { get; private set; }

    public Customer Customer { get; private set; } = null!;

    public Deal? Deal { get; private set; }

    public Provider? Provider { get; private set; }

    public Request Request { get; private set; } = null!;

    private Complaint()
    {
    }

    public Complaint(long requestId, long customerId, string type, string description)
    {
        RequestId = requestId;
        CustomerId = customerId;
        Type = type;
        Description = description;
    }
}

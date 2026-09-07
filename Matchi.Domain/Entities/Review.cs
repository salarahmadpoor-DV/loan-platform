using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Review : AuditableEntity
{
    public long DealId { get; private set; };

    public long CustomerId { get; private set; };

    public long? BusinessId { get; private set; };

    public long? ProviderId { get; private set; };

    public byte Rating { get; private set; };

    public string? Comment { get; private set; };

    public Business? Business { get; private set; }

    public Customer Customer { get; private set; } = null!;

    public Deal Deal { get; private set; } = null!;

    public Provider? Provider { get; private set; }

    private Review()
    {
    }

    public Review(long dealId, long customerId, byte rating)
    {
        DealId = dealId;
        CustomerId = customerId;
        Rating = rating;
    }
}

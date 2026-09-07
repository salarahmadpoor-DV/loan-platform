using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Proposal : AuditableEntity
{
    public long RequestId { get; private set; }

    public long? BusinessId { get; private set; }

    public long? ProviderId { get; private set; }

    public decimal TotalPrice { get; private set; }

    public decimal DeliveryFee { get; private set; } = 0m;

    public string? Message { get; private set; }

    public DateOnly? ProposedDate { get; private set; }

    public TimeSpan? ProposedTimeFrom { get; private set; }

    public TimeSpan? ProposedTimeTo { get; private set; }

    public string Status { get; private set; } = "Pending";

    public DateTime? ExpireAt { get; private set; }

    public Deal? Deal { get; private set; }

    public Business? Business { get; private set; }

    public Provider? Provider { get; private set; }

    public Request Request { get; private set; } = null!;

    public ICollection<ProposalItem> Items { get; private set; } = new List<ProposalItem>();

    private Proposal()
    {
    }

    public Proposal(long requestId, decimal totalPrice)
    {
        RequestId = requestId;
        TotalPrice = totalPrice;
    }
}

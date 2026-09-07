using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProposalItem : TimestampedEntity
{
    public long ProposalId { get; private set; }

    public string ItemType { get; private set; } = null!;

    public long? ProductId { get; private set; }

    public long? ServiceId { get; private set; }

    public string? Description { get; private set; }

    public decimal Quantity { get; private set; } = 1m;

    public decimal UnitPrice { get; private set; }

    public decimal TotalPrice { get; private set; }

    public int DisplayOrder { get; private set; } = 0;

    public Product? Product { get; private set; }

    public Proposal Proposal { get; private set; } = null!;

    public Service? Service { get; private set; }

    private ProposalItem()
    {
    }

    public ProposalItem(long proposalId, string itemType, decimal unitPrice, decimal totalPrice)
    {
        ProposalId = proposalId;
        ItemType = itemType;
        UnitPrice = unitPrice;
        TotalPrice = totalPrice;
    }
}

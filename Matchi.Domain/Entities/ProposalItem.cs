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

    private ProposalItem(
        string itemType,
        long? productId,
        long? serviceId,
        string? description,
        decimal quantity,
        decimal unitPrice,
        decimal totalPrice,
        int displayOrder)
    {
        ItemType = itemType;
        ProductId = productId;
        ServiceId = serviceId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = totalPrice;
        DisplayOrder = displayOrder;
    }

    public static ProposalItem ForService(
        long serviceId,
        decimal quantity,
        decimal unitPrice,
        decimal totalPrice,
        string? description = null,
        int displayOrder = 0)
    {
        return new ProposalItem("Service", null, serviceId, description, quantity, unitPrice, totalPrice, displayOrder);
    }

    public static ProposalItem ForProduct(
        long productId,
        decimal quantity,
        decimal unitPrice,
        decimal totalPrice,
        string? description = null,
        int displayOrder = 0)
    {
        return new ProposalItem("Product", productId, null, description, quantity, unitPrice, totalPrice, displayOrder);
    }
}

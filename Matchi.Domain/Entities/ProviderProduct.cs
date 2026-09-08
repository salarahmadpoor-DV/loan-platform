using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProviderProduct : AuditableEntity
{
    public long ProviderId { get; private set; }

    public long ProductId { get; private set; }

    public decimal? Price { get; private set; }

    public bool IsAvailable { get; private set; } = true;

    public decimal? MinOrderQuantity { get; private set; }

    public int? LeadTimeDays { get; private set; }

    public Product Product { get; private set; } = null!;

    public Provider Provider { get; private set; } = null!;

    private ProviderProduct()
    {
    }

    public ProviderProduct(long providerId, long productId)
    {
        ProviderId = providerId;
        ProductId = productId;
        IsAvailable = true;
    }

    public void UpdateOffer(
        decimal? price,
        bool isAvailable,
        decimal? minOrderQuantity,
        int? leadTimeDays)
    {
        Price = price;
        IsAvailable = isAvailable;
        MinOrderQuantity = minOrderQuantity;
        LeadTimeDays = leadTimeDays;
        SetUpdated();
    }

    public void Reactivate(
        decimal? price,
        bool isAvailable,
        decimal? minOrderQuantity,
        int? leadTimeDays)
    {
        Restore();
        UpdateOffer(price, isAvailable, minOrderQuantity, leadTimeDays);
    }
}

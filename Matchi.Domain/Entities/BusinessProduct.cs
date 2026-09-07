using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class BusinessProduct : AuditableEntity
{
    public long BusinessId { get; private set; }

    public long ProductId { get; private set; }

    public decimal? Price { get; private set; }

    public bool IsAvailable { get; private set; } = true;

    public decimal? MinOrderQuantity { get; private set; }

    public int? LeadTimeDays { get; private set; }

    public Business Business { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    private BusinessProduct()
    {
    }

    public BusinessProduct(long businessId, long productId)
    {
        BusinessId = businessId;
        ProductId = productId;
    }
}

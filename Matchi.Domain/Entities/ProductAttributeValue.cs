using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProductAttributeValue : AuditableEntity
{
    public long ProductId { get; private set; };

    public long ProductAttributeId { get; private set; };

    public string Value { get; private set; } = null!;

    public ProductAttribute ProductAttribute { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    private ProductAttributeValue()
    {
    }

    public ProductAttributeValue(long productId, long productAttributeId, string value)
    {
        ProductId = productId;
        ProductAttributeId = productAttributeId;
        Value = value;
    }
}

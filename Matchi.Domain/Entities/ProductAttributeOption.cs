using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProductAttributeOption : AuditableEntity
{
    public long ProductAttributeId { get; private set; }

    public string Value { get; private set; } = null!;

    public string DisplayName { get; private set; } = null!;

    public int DisplayOrder { get; private set; } = 0;

    public bool IsActive { get; private set; } = true;

    public ProductAttribute ProductAttribute { get; private set; } = null!;

    private ProductAttributeOption()
    {
    }

    public ProductAttributeOption(long productAttributeId, string value, string displayName)
    {
        ProductAttributeId = productAttributeId;
        Value = value;
        DisplayName = displayName;
    }
}

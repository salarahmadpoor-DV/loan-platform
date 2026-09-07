using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProductAttribute : AuditableEntity
{
    public long ProductCategoryId { get; private set; }

    public string Name { get; private set; } = null!;

    public string Code { get; private set; } = null!;

    public string DataType { get; private set; } = null!;

    public bool IsRequired { get; private set; } = false;

    public int DisplayOrder { get; private set; } = 0;

    public bool IsActive { get; private set; } = true;

    public ProductCategory ProductCategory { get; private set; } = null!;

    public ICollection<ProductAttributeOption> Options { get; private set; } = new List<ProductAttributeOption>();

    public ICollection<ProductAttributeValue> Values { get; private set; } = new List<ProductAttributeValue>();

    public ICollection<RequestProductAttribute> RequestProductAttributes { get; private set; } = new List<RequestProductAttribute>();

    private ProductAttribute()
    {
    }

    public ProductAttribute(long productCategoryId, string name, string code, string dataType)
    {
        ProductCategoryId = productCategoryId;
        Name = name;
        Code = code;
        DataType = dataType;
    }
}

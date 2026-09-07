using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProductCategory : AuditableEntity
{
    public long? ParentId { get; private set; };

    public string Name { get; private set; } = null!;

    public string Slug { get; private set; } = null!;

    public string? Description { get; private set; };

    public int DisplayOrder { get; private set; } = 0;

    public bool IsActive { get; private set; } = true;

    public ProductCategory? Parent { get; private set; }

    public ICollection<ProductAttribute> Attributes { get; private set; } = new List<ProductAttribute>();

    public ICollection<ProductCategory> Children { get; private set; } = new List<ProductCategory>();

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    public ICollection<RequestProduct> RequestProducts { get; private set; } = new List<RequestProduct>();

    private ProductCategory()
    {
    }

    public ProductCategory(string name, string slug)
    {
        Name = name;
        Slug = slug;
    }
}

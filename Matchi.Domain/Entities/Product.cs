using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Product : AuditableEntity
{
    public long CategoryId { get; private set; };

    public string Name { get; private set; } = null!;

    public string? Slug { get; private set; };

    public string? Description { get; private set; };

    public string? Brand { get; private set; };

    public string? Model { get; private set; };

    public string? SKU { get; private set; };

    public bool IsActive { get; private set; } = true;

    public ProductCategory Category { get; private set; } = null!;

    public ICollection<BusinessProduct> BusinessProducts { get; private set; } = new List<BusinessProduct>();

    public ICollection<ProductAttributeValue> AttributeValues { get; private set; } = new List<ProductAttributeValue>();

    public ICollection<ProposalItem> ProposalItems { get; private set; } = new List<ProposalItem>();

    public ICollection<ProviderProduct> ProviderProducts { get; private set; } = new List<ProviderProduct>();

    public ICollection<RequestProduct> RequestProducts { get; private set; } = new List<RequestProduct>();

    private Product()
    {
    }

    public Product(long categoryId, string name)
    {
        CategoryId = categoryId;
        Name = name;
    }
}

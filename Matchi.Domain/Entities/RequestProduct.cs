using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class RequestProduct : AuditableEntity
{
    public long RequestId { get; private set; };

    public long? ProductId { get; private set; };

    public long? ProductCategoryId { get; private set; };

    public decimal Quantity { get; private set; } = 1m;

    public string? Unit { get; private set; };

    public string? Description { get; private set; };

    public int DisplayOrder { get; private set; } = 0;

    public ProductCategory? ProductCategory { get; private set; }

    public Product? Product { get; private set; }

    public Request Request { get; private set; } = null!;

    public ICollection<RequestProductAttribute> Attributes { get; private set; } = new List<RequestProductAttribute>();

    private RequestProduct()
    {
    }

    public RequestProduct(long requestId)
    {
        RequestId = requestId;
    }
}

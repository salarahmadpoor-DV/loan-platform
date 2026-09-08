using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class RequestProductAttribute : AuditableEntity
{
    public long RequestProductId { get; private set; }

    public long ProductAttributeId { get; private set; }

    public string? Value { get; private set; }

    public ProductAttribute ProductAttribute { get; private set; } = null!;

    public RequestProduct RequestProduct { get; private set; } = null!;

    private RequestProductAttribute()
    {
    }

    public RequestProductAttribute(long requestProductId, long productAttributeId, string? value = null)
    {
        RequestProductId = requestProductId;
        ProductAttributeId = productAttributeId;
        Value = value;
    }
}

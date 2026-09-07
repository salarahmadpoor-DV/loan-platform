using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class RequestServiceAttribute : AuditableEntity
{
    public long RequestServiceId { get; private set; }

    public long ServiceAttributeId { get; private set; }

    public string? Value { get; private set; }

    public RequestService RequestService { get; private set; } = null!;

    public ServiceAttribute ServiceAttribute { get; private set; } = null!;

    private RequestServiceAttribute()
    {
    }

    public RequestServiceAttribute(long requestServiceId, long serviceAttributeId, string? value = null)
    {
        RequestServiceId = requestServiceId;
        ServiceAttributeId = serviceAttributeId;
        Value = value;
    }
}

using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class RequestService : AuditableEntity
{
    public long RequestId { get; private set; }

    public long ServiceId { get; private set; }

    public decimal Quantity { get; private set; } = 1m;

    public string? Description { get; private set; }

    public int DisplayOrder { get; private set; } = 0;

    public Request Request { get; private set; } = null!;

    public Service Service { get; private set; } = null!;

    public ICollection<RequestServiceAttribute> Attributes { get; private set; } = new List<RequestServiceAttribute>();

    private RequestService()
    {
    }

    public RequestService(long requestId, long serviceId)
    {
        RequestId = requestId;
        ServiceId = serviceId;
    }
}

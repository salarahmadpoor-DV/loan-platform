using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProviderService : AuditableEntity
{
    public long ProviderId { get; private set; };

    public long ServiceId { get; private set; };

    public bool IsActive { get; private set; } = true;

    public Provider Provider { get; private set; } = null!;

    public Service Service { get; private set; } = null!;

    private ProviderService()
    {
    }

    public ProviderService(long providerId, long serviceId)
    {
        ProviderId = providerId;
        ServiceId = serviceId;
        IsActive = true;
    }
}

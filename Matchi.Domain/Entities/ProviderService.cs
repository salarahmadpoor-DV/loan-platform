using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProviderService : Entity
{
    public long ProviderId { get; private set; }
    public Provider Provider { get; private set; } = null!;

    public long ServiceId { get; private set; }
    public Service Service { get; private set; } = null!;

    private ProviderService() { }

    public ProviderService(long providerId, long serviceId)
    {
        ProviderId = providerId;
        ServiceId = serviceId;
    }
}
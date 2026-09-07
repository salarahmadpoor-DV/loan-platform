using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProviderCapability : AuditableEntity
{
    public long ProviderId { get; private set; };

    public long ServiceAttributeId { get; private set; };

    public string Value { get; private set; } = null!;

    public Provider Provider { get; private set; } = null!;

    public ServiceAttribute ServiceAttribute { get; private set; } = null!;

    private ProviderCapability()
    {
    }

    public ProviderCapability(long providerId, long serviceAttributeId, string value)
    {
        ProviderId = providerId;
        ServiceAttributeId = serviceAttributeId;
        Value = value;
    }
}

using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ServiceAttributeOption : AuditableEntity
{
    public long ServiceAttributeId { get; private set; }

    public string Value { get; private set; } = null!;

    public string DisplayName { get; private set; } = null!;

    public int DisplayOrder { get; private set; } = 0;

    public bool IsActive { get; private set; } = true;

    public ServiceAttribute ServiceAttribute { get; private set; } = null!;

    private ServiceAttributeOption()
    {
    }

    public ServiceAttributeOption(long serviceAttributeId, string value, string displayName)
    {
        ServiceAttributeId = serviceAttributeId;
        Value = value;
        DisplayName = displayName;
    }
}

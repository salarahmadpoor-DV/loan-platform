using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ServiceAttribute : AuditableEntity
{
    public long ServiceId { get; private set; };

    public string Name { get; private set; } = null!;

    public string Code { get; private set; } = null!;

    public string DataType { get; private set; } = null!;

    public bool IsRequired { get; private set; } = 0;

    public int DisplayOrder { get; private set; } = 0;

    public bool IsActive { get; private set; } = true;

    public Service Service { get; private set; } = null!;

    public ICollection<ProviderCapability> ProviderCapabilities { get; private set; } = new List<ProviderCapability>();

    public ICollection<RequestServiceAttribute> RequestServiceAttributes { get; private set; } = new List<RequestServiceAttribute>();

    public ICollection<ServiceAttributeOption> Options { get; private set; } = new List<ServiceAttributeOption>();

    private ServiceAttribute()
    {
    }

    public ServiceAttribute(long serviceId, string name, string code, string dataType)
    {
        ServiceId = serviceId;
        Name = name;
        Code = code;
        DataType = dataType;
    }
}

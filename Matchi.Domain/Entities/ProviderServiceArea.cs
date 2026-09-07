using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProviderServiceArea : AuditableEntity
{
    public long ProviderId { get; private set; }

    public string AreaType { get; private set; } = null!;

    public string? Province { get; private set; }

    public string? City { get; private set; }

    public string? District { get; private set; }

    public decimal? Lat { get; private set; }

    public decimal? Lng { get; private set; }

    public decimal? Radius { get; private set; }

    public bool IsActive { get; private set; } = true;

    public Provider Provider { get; private set; } = null!;

    private ProviderServiceArea()
    {
    }

    public ProviderServiceArea(long providerId, string areaType)
    {
        ProviderId = providerId;
        AreaType = areaType;
    }
}

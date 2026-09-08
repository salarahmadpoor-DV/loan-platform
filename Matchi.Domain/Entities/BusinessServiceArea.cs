using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class BusinessServiceArea : AuditableEntity
{
    public long BusinessId { get; private set; }

    public string AreaType { get; private set; } = null!;

    public string? Province { get; private set; }

    public string? City { get; private set; }

    public string? District { get; private set; }

    public decimal? Lat { get; private set; }

    public decimal? Lng { get; private set; }

    public decimal? Radius { get; private set; }

    public bool IsActive { get; private set; } = true;

    public Business Business { get; private set; } = null!;

    private BusinessServiceArea()
    {
    }

    public BusinessServiceArea(long businessId, string areaType)
    {
        BusinessId = businessId;
        AreaType = areaType;
        IsActive = true;
    }

    public void UpdateArea(
        string areaType,
        string? province,
        string? city,
        string? district,
        decimal? lat,
        decimal? lng,
        decimal? radius,
        bool isActive)
    {
        AreaType = areaType;
        Province = province;
        City = city;
        District = district;
        Lat = lat;
        Lng = lng;
        Radius = radius;
        IsActive = isActive;
        SetUpdated();
    }
}

using Matchi.Domain.Common;

namespace Matchi.Domain.Locations;

public class LocationDistrict : Entity
{
    public long CityId { get; private set; }

    public string Name { get; private set; } = null!;

    public string Code { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    public DateTime CreateDate { get; private set; }

    public decimal? CenterLat { get; private set; }

    public decimal? CenterLng { get; private set; }

    public decimal? RadiusKm { get; private set; }

    public LocationCity City { get; private set; } = null!;

    private LocationDistrict()
    {
    }

    public LocationDistrict(long cityId, string name, string code = "")
    {
        CityId = cityId;
        Name = name;
        Code = code;
        IsActive = true;
        CreateDate = DateTime.UtcNow;
    }

    public void SetCoverage(decimal centerLat, decimal centerLng, decimal radiusKm)
    {
        CenterLat = centerLat;
        CenterLng = centerLng;
        RadiusKm = radiusKm;
    }
}

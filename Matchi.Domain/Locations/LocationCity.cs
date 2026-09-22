using Matchi.Domain.Common;

namespace Matchi.Domain.Locations;

public class LocationCity : Entity
{
    public long ProvinceId { get; private set; }

    public string Name { get; private set; } = null!;

    public string Code { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    public DateTime CreateDate { get; private set; }

    public decimal? CenterLat { get; private set; }

    public decimal? CenterLng { get; private set; }

    public decimal? RadiusKm { get; private set; }

    public LocationProvince Province { get; private set; } = null!;

    public ICollection<LocationDistrict> Districts { get; private set; } = new List<LocationDistrict>();

    private LocationCity()
    {
    }

    public LocationCity(long provinceId, string name, string code = "")
    {
        ProvinceId = provinceId;
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

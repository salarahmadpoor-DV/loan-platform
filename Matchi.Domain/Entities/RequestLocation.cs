using Matchi.Domain.Common;
using Matchi.Domain.Locations;

namespace Matchi.Domain.Entities;

public class RequestLocation : TimestampedEntity
{
    public long RequestId { get; private set; }

    public long? ProvinceId { get; private set; }

    public long? CityId { get; private set; }

    public long? DistrictId { get; private set; }

    public string? Address { get; private set; }

    public decimal? Lat { get; private set; }

    public decimal? Lng { get; private set; }

    public Request Request { get; private set; } = null!;

    public LocationProvince Province { get; private set; } = null!;

    public LocationCity City { get; private set; } = null!;

    public LocationDistrict District { get; private set; } = null!;

    private RequestLocation()
    {
    }

    public RequestLocation(
        long requestId,
        long provinceId,
        long cityId,
        long districtId,
        string? address = null,
        decimal? lat = null,
        decimal? lng = null)
    {
        RequestId = requestId;
        ProvinceId = provinceId;
        CityId = cityId;
        DistrictId = districtId;
        Address = address;
        Lat = lat;
        Lng = lng;
    }
}

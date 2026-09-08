using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class RequestLocation : TimestampedEntity
{
    public long RequestId { get; private set; }

    public string? Address { get; private set; }

    public string? Province { get; private set; }

    public string? City { get; private set; }

    public string? District { get; private set; }

    public decimal? Lat { get; private set; }

    public decimal? Lng { get; private set; }

    public Request Request { get; private set; } = null!;

    private RequestLocation()
    {
    }

    public RequestLocation(
        long requestId,
        string? province = null,
        string? city = null,
        string? district = null,
        string? address = null,
        decimal? lat = null,
        decimal? lng = null)
    {
        RequestId = requestId;
        Province = province;
        City = city;
        District = district;
        Address = address;
        Lat = lat;
        Lng = lng;
    }
}

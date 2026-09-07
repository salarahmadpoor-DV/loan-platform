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

    public RequestLocation(long requestId, decimal? lat = null, decimal? lng = null, string? address = null)
    {
        RequestId = requestId;
        Lat = lat;
        Lng = lng;
        Address = address;
    }
}

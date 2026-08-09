using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ServiceRequest : AuditableEntity
{
    public long UserId { get; private set; }
    public long ServiceId { get; private set; }

    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }

    public double? Lat { get; private set; }
    public double? Lng { get; private set; }

    public string Status { get; private set; } = "Created";

    public ICollection<RequestAnswer> Answers { get; private set; } = new List<RequestAnswer>();
    public ICollection<Introduction> Introductions { get; private set; } = new List<Introduction>();

    private ServiceRequest() { }

    public ServiceRequest(long userId, long serviceId, string title, string? description, double? lat = null, double? lng = null)
    {
        UserId = userId;
        ServiceId = serviceId;
        Title = title;
        Description = description;
        Lat = lat;
        Lng = lng;
    }
}
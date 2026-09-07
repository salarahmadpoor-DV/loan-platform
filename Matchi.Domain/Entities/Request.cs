using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Request : AuditableEntity
{
    public long CustomerId { get; private set; };

    public string RequestType { get; private set; } = null!;

    public string Title { get; private set; } = null!;

    public string? Description { get; private set; };

    public string Status { get; private set; } = "Open";

    public Customer Customer { get; private set; } = null!;

    public ICollection<Cancellation> Cancellations { get; private set; } = new List<Cancellation>();

    public ICollection<Complaint> Complaints { get; private set; } = new List<Complaint>();

    public ICollection<Conversation> Conversations { get; private set; } = new List<Conversation>();

    public ICollection<Deal> Deals { get; private set; } = new List<Deal>();

    public ICollection<Proposal> Proposals { get; private set; } = new List<Proposal>();

    public ICollection<RequestLocation> Locations { get; private set; } = new List<RequestLocation>();

    public ICollection<RequestProduct> Products { get; private set; } = new List<RequestProduct>();

    public ICollection<RequestSchedule> Schedules { get; private set; } = new List<RequestSchedule>();

    public ICollection<RequestService> Services { get; private set; } = new List<RequestService>();

    private Request()
    {
    }

    public Request(long customerId, string requestType, string title)
    {
        CustomerId = customerId;
        RequestType = requestType;
        Title = title;
    }
}

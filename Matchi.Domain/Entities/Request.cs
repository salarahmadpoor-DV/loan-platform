using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Request : AuditableEntity
{
    public long CustomerId { get; private set; }

    public string RequestType { get; private set; } = null!;

    public string Title { get; private set; } = null!;

    public string? Description { get; private set; }

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

    public Request(long customerId, string requestType, string title, string? description = null)
    {
        CustomerId = customerId;
        RequestType = requestType;
        Title = title;
        Description = description;
        Status = "Open";
    }

    public void UpdateDetails(string requestType, string title, string? description)
    {
        RequestType = requestType;
        Title = title;
        Description = description;
        SetUpdated();
    }

    public bool CanBeModified() => !IsDeleted && Status == "Open";

    public void Cancel()
    {
        Status = "Cancelled";
        SetUpdated();
    }

    public void RetireLineItems()
    {
        foreach (var service in Services)
            service.Retire();

        foreach (var product in Products)
            product.Retire();

        SetUpdated();
    }

    public void AddService(RequestService service) => Services.Add(service);

    public void AddProduct(RequestProduct product) => Products.Add(product);

    public void AddLocation(RequestLocation location) => Locations.Add(location);

    public void AddSchedule(RequestSchedule schedule) => Schedules.Add(schedule);
}

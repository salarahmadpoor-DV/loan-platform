using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Provider : AuditableEntity
{
    public long UserId { get; private set; }

    public string Name { get; private set; } = null!;

    public string Mobile { get; private set; } = null!;

    public string? Description { get; private set; }

    public decimal? Lat { get; private set; }

    public decimal? Lng { get; private set; }

    public decimal Rating { get; private set; } = 0m;

    public int ReviewCount { get; private set; } = 0;

    public int CompletedJobCount { get; private set; } = 0;

    public string Status { get; private set; } = "Active";

    public User User { get; private set; } = null!;

    public ICollection<BusinessProvider> BusinessProviders { get; private set; } = new List<BusinessProvider>();

    public ICollection<Complaint> Complaints { get; private set; } = new List<Complaint>();

    public ICollection<ExecutionAssignment> ExecutionAssignments { get; private set; } = new List<ExecutionAssignment>();

    public ICollection<Proposal> Proposals { get; private set; } = new List<Proposal>();

    public ICollection<ProviderAvailability> Availabilities { get; private set; } = new List<ProviderAvailability>();

    public ICollection<ProviderCapability> Capabilities { get; private set; } = new List<ProviderCapability>();

    public ICollection<ProviderPortfolio> Portfolios { get; private set; } = new List<ProviderPortfolio>();

    public ICollection<ProviderProduct> Products { get; private set; } = new List<ProviderProduct>();

    public ICollection<ProviderServiceArea> ServiceAreas { get; private set; } = new List<ProviderServiceArea>();

    public ICollection<ProviderService> ProviderServices { get; private set; } = new List<ProviderService>();

    public ICollection<Review> Reviews { get; private set; } = new List<Review>();

    private Provider()
    {
    }

    public Provider(long userId, string name, string mobile, decimal? lat = null, decimal? lng = null, string? description = null)
    {
        UserId = userId;
        Name = name;
        Mobile = mobile;
        Lat = lat;
        Lng = lng;
        Description = description;
        Rating = 0m;
        ReviewCount = 0;
        CompletedJobCount = 0;
        Status = "Active";
    }
}

using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Business : AuditableEntity
{
    public long OwnerUserId { get; private set; };

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; };

    public string? Mobile { get; private set; };

    public string? Address { get; private set; };

    public string? Province { get; private set; };

    public string? City { get; private set; };

    public string? District { get; private set; };

    public decimal? Lat { get; private set; };

    public decimal? Lng { get; private set; };

    public long? LogoMediaId { get; private set; };

    public decimal Rating { get; private set; } = 0m;

    public int ReviewCount { get; private set; } = 0;

    public int CompletedJobCount { get; private set; } = 0;

    public string Status { get; private set; } = "Active";

    public Media? LogoMedia { get; private set; }

    public User OwnerUser { get; private set; } = null!;

    public ICollection<BusinessAvailability> Availabilities { get; private set; } = new List<BusinessAvailability>();

    public ICollection<BusinessPortfolio> Portfolios { get; private set; } = new List<BusinessPortfolio>();

    public ICollection<BusinessProduct> Products { get; private set; } = new List<BusinessProduct>();

    public ICollection<BusinessProvider> BusinessProviders { get; private set; } = new List<BusinessProvider>();

    public ICollection<BusinessServiceArea> ServiceAreas { get; private set; } = new List<BusinessServiceArea>();

    public ICollection<BusinessService> Services { get; private set; } = new List<BusinessService>();

    public ICollection<Complaint> Complaints { get; private set; } = new List<Complaint>();

    public ICollection<Conversation> Conversations { get; private set; } = new List<Conversation>();

    public ICollection<Proposal> Proposals { get; private set; } = new List<Proposal>();

    public ICollection<Review> Reviews { get; private set; } = new List<Review>();

    public ICollection<ServiceExecution> ServiceExecutions { get; private set; } = new List<ServiceExecution>();

    private Business()
    {
    }

    public Business(long ownerUserId, string name, string? address = null, decimal? lat = null, decimal? lng = null)
    {
        OwnerUserId = ownerUserId;
        Name = name;
        Address = address;
        Lat = lat;
        Lng = lng;
        Rating = 0m;
        ReviewCount = 0;
        CompletedJobCount = 0;
        Status = "Active";
    }
}

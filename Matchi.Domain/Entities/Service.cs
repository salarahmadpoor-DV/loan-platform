using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Service : AuditableEntity
{
    public long CategoryId { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Slug { get; private set; }

    public string? Description { get; private set; }

    public int DisplayOrder { get; private set; } = 0;

    public bool IsActive { get; private set; } = true;

    public ServiceCategory Category { get; private set; } = null!;

    public ICollection<BusinessService> BusinessServices { get; private set; } = new List<BusinessService>();

    public ICollection<ProposalItem> ProposalItems { get; private set; } = new List<ProposalItem>();

    public ICollection<ProviderService> ProviderServices { get; private set; } = new List<ProviderService>();

    public ICollection<RequestService> RequestServices { get; private set; } = new List<RequestService>();

    public ICollection<ServiceAttribute> Attributes { get; private set; } = new List<ServiceAttribute>();

    private Service()
    {
    }

    public Service(string name, long categoryId, string? slug = null, string? description = null, int displayOrder = 0)
    {
        Name = name;
        CategoryId = categoryId;
        Slug = slug;
        Description = description;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
}

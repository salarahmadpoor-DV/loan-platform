using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ServiceCategory : AuditableEntity
{
    public string Name { get; private set; } = null!;

    public string Slug { get; private set; } = null!;

    public int DisplayOrder { get; private set; } = 0;

    public bool IsActive { get; private set; } = true;

    public ICollection<Service> Services { get; private set; } = new List<Service>();

    private ServiceCategory()
    {
    }

    public ServiceCategory(string name, string slug, int displayOrder = 0)
    {
        Name = name;
        Slug = slug;
        DisplayOrder = displayOrder;
        IsActive = true;
    }
}

using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ServiceCategory : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;

    public ICollection<Service> Services { get; private set; } = new List<Service>();

    private ServiceCategory() { }

    public ServiceCategory(string name, string slug)
    {
        Name = name;
        Slug = slug;
    }
}
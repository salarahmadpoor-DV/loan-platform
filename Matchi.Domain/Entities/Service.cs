using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Service : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public long CategoryId { get; private set; }
    public ServiceCategory Category { get; private set; } = null!;

    public ICollection<ServiceQuestion> Questions { get; private set; } = new List<ServiceQuestion>();
    public ICollection<ServiceRequest> Requests { get; private set; } = new List<ServiceRequest>();

    private Service() { }

    public Service(string name, long categoryId)
    {
        Name = name;
        CategoryId = categoryId;
    }
}
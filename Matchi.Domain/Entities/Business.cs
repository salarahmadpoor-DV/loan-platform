using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Business : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string? Address { get; private set; }
    public long OwnerUserId { get; private set; }

    public User OwnerUser { get; private set; } = null!;
    public double? Lat { get; private set; }
    public double? Lng { get; private set; }

    public ICollection<BusinessProvider> BusinessProviders { get; private set; } = new List<BusinessProvider>();

    private Business() { }

    public Business(string name, string? address = null, double? lat = null, double? lng = null)
    {
        Name = name;
        Address = address;
        Lat = lat;
        Lng = lng;
    }
}
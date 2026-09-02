using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Provider : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Mobile { get; private set; } = null!;

    public double? Lat { get; private set; }
    public double? Lng { get; private set; }

    public long? UserId { get; private set; }
    public User? User { get; private set; }

    public double Rating { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<ProviderService> ProviderServices { get; private set; }
        = new List<ProviderService>();

    private Provider()
    {
    }

    public Provider(
        string name,
        string mobile,
        double? lat = null,
        double? lng = null)
    {
        Name = name;
        Mobile = mobile;
        Lat = lat;
        Lng = lng;
        Rating = 0;
        IsActive = true;
    }

    public void SetUser(long userId)
    {
        UserId = userId;
    }
}
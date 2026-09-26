using Matchi.Domain.Common;

namespace Matchi.Domain.Locations;

public class LocationProvince : Entity
{
    public string Name { get; private set; } = null!;

    public string Code { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    public DateTime CreateDate { get; private set; }

    public ICollection<LocationCity> Cities { get; private set; } = new List<LocationCity>();

    private LocationProvince()
    {
    }

    public LocationProvince(string name, string code)
    {
        Name = name;
        Code = code;
        IsActive = true;
        CreateDate = DateTime.UtcNow;
    }
}

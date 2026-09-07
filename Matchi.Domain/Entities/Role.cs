using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Role : TimestampedEntity
{
    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; };

    public bool IsActive { get; set; } = true;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

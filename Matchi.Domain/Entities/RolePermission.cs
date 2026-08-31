namespace Matchi.Domain.Entities;

public class RolePermission
{
    public long RoleId { get; set; }

    public long PermissionId { get; set; }

    public DateTime CreateDate { get; set; }

    // Navigation Properties
    public Role Role { get; set; } = null!;

    public Permission Permission { get; set; } = null!;
}
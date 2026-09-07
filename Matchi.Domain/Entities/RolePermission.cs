using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class RolePermission
{
    public long RoleId { get; set; };

    public long PermissionId { get; set; };

    public DateTime CreateDate { get; set; };

    public Permission Permission { get; set; } = null!;

    public Role Role { get; set; } = null!;
}

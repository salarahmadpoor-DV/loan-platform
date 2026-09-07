using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class UserRole
{
    public long UserId { get; set; };

    public long RoleId { get; set; };

    public DateTime CreateDate { get; set; };

    public Role Role { get; set; } = null!;

    public User User { get; set; } = null!;
}

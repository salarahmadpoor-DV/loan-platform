namespace Matchi.Domain.Entities;

public class UserRole
{
    public long UserId { get; set; }

    public long RoleId { get; set; }

    public DateTime CreateDate { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;

    public Role Role { get; set; } = null!;
}
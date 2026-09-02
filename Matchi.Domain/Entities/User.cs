using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class User : AuditableEntity
{
    public string Mobile { get; private set; } = null!;

    public string? Name { get; private set; }

    public bool IsMobileVerified { get; private set; }

    public ICollection<LoanRequest> LoanRequests { get; private set; } = new List<LoanRequest>();

    private User()
    {
    }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public User(string mobile)
    {
        Mobile = mobile;
        IsMobileVerified = false;
    }

    public void VerifyMobile()
    {
        IsMobileVerified = true;
        SetUpdated();
    }

    public void UpdateProfile(string? name)
    {
        Name = name;
        SetUpdated();
    }

    public Provider? Provider { get; private set; }

    public ICollection<Business> Businesses { get; private set; }
    = new List<Business>();
}

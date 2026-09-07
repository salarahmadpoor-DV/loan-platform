# Generates Matchi domain entities and EF configurations from the SQL Server baseline.
from pathlib import Path

ROOT = Path(r"E:\Armin\Matchi\Matchi-platform")
ENT = ROOT / "Matchi.Domain" / "Entities"
CFG = ROOT / "Matchi.Infrastructure" / "Persistence" / "Configurations"

HDR_ENT = """using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;
"""

HDR_CFG = """using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;
"""

def write(path: Path, content: str):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content.replace("\r\n", "\n"), encoding="utf-8")

# ---------------------------------------------------------------------------
# Domain entities
# ---------------------------------------------------------------------------

write(ENT / "User.cs", HDR_ENT + """
public class User : AuditableEntity
{
    public string Mobile { get; private set; } = null!;
    public string? Name { get; private set; }
    public bool IsMobileVerified { get; private set; }

    public Provider? Provider { get; private set; }
    public Customer? Customer { get; private set; }
    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<Business> Businesses { get; private set; } = new List<Business>();
    public ICollection<ConversationParticipant> ConversationParticipants { get; private set; } = new List<ConversationParticipant>();
    public ICollection<Message> SentMessages { get; private set; } = new List<Message>();
    public ICollection<Cancellation> Cancellations { get; private set; } = new List<Cancellation>();

    private User()
    {
    }

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
}
""")

write(ENT / "Customer.cs", HDR_ENT + """
public class Customer : AuditableEntity
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public ICollection<Request> Requests { get; private set; } = new List<Request>();
    public ICollection<Deal> Deals { get; private set; } = new List<Deal>();
    public ICollection<Review> Reviews { get; private set; } = new List<Review>();
    public ICollection<Complaint> Complaints { get; private set; } = new List<Complaint>();
    public ICollection<Conversation> Conversations { get; private set; } = new List<Conversation>();

    private Customer()
    {
    }

    public Customer(long userId)
    {
        UserId = userId;
    }
}
""")

write(ENT / "Role.cs", """namespace Matchi.Domain.Entities;

public class Role : Matchi.Domain.Common.TimestampedEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
""")

write(ENT / "Permission.cs", """namespace Matchi.Domain.Entities;

public class Permission : Matchi.Domain.Common.TimestampedEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
""")

write(ENT / "UserRole.cs", """namespace Matchi.Domain.Entities;

public class UserRole
{
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public DateTime CreateDate { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
""")

write(ENT / "RolePermission.cs", """namespace Matchi.Domain.Entities;

public class RolePermission
{
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
    public DateTime CreateDate { get; set; }

    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
""")

write(ENT / "Provider.cs", HDR_ENT + """
public class Provider : AuditableEntity
{
    public long UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Mobile { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal? Lat { get; private set; }
    public decimal? Lng { get; private set; }
    public decimal Rating { get; private set; }
    public int ReviewCount { get; private set; }
    public int CompletedJobCount { get; private set; }
    public string Status { get; private set; } = "Active";

    public User User { get; private set; } = null!;
    public ICollection<BusinessProvider> BusinessProviders { get; private set; } = new List<BusinessProvider>();
    public ICollection<ProviderService> ProviderServices { get; private set; } = new List<ProviderService>();
    public ICollection<ProviderCapability> ProviderCapabilities { get; private set; } = new List<ProviderCapability>();
    public ICollection<ProviderProduct> ProviderProducts { get; private set; } = new List<ProviderProduct>();
    public ICollection<ProviderServiceArea> ProviderServiceAreas { get; private set; } = new List<ProviderServiceArea>();
    public ICollection<ProviderAvailability> ProviderAvailabilities { get; private set; } = new List<ProviderAvailability>();
    public ICollection<ProviderPortfolio> ProviderPortfolios { get; private set; } = new List<ProviderPortfolio>();
    public ICollection<Proposal> Proposals { get; private set; } = new List<Proposal>();
    public ICollection<ExecutionAssignment> ExecutionAssignments { get; private set; } = new List<ExecutionAssignment>();
    public ICollection<Review> Reviews { get; private set; } = new List<Review>();
    public ICollection<Complaint> Complaints { get; private set; } = new List<Complaint>();

    private Provider()
    {
    }

    public Provider(long userId, string name, string mobile, decimal? lat = null, decimal? lng = null, string? description = null)
    {
        UserId = userId;
        Name = name;
        Mobile = mobile;
        Lat = lat;
        Lng = lng;
        Description = description;
        Rating = 0;
        ReviewCount = 0;
        CompletedJobCount = 0;
        Status = "Active";
    }
}
""")

write(ENT / "Business.cs", HDR_ENT + """
public class Business : AuditableEntity
{
    public long OwnerUserId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Mobile { get; private set; }
    public string? Address { get; private set; }
    public string? Province { get; private set; }
    public string? City { get; private set; }
    public string? District { get; private set; }
    public decimal? Lat { get; private set; }
    public decimal? Lng { get; private set; }
    public long? LogoMediaId { get; private set; }
    public decimal Rating { get; private set; }
    public int ReviewCount { get; private set; }
    public int CompletedJobCount { get; private set; }
    public string Status { get; private set; } = "Active";

    public User OwnerUser { get; private set; } = null!;
    public Media? LogoMedia { get; private set; }
    public ICollection<BusinessProvider> BusinessProviders { get; private set; } = new List<BusinessProvider>();
    public ICollection<BusinessService> BusinessServices { get; private set; } = new List<BusinessService>();
    public ICollection<BusinessProduct> BusinessProducts { get; private set; } = new List<BusinessProduct>();
    public ICollection<BusinessServiceArea> BusinessServiceAreas { get; private set; } = new List<BusinessServiceArea>();
    public ICollection<BusinessAvailability> BusinessAvailabilities { get; private set; } = new List<BusinessAvailability>();
    public ICollection<BusinessPortfolio> BusinessPortfolios { get; private set; } = new List<BusinessPortfolio>();
    public ICollection<Proposal> Proposals { get; private set; } = new List<Proposal>();
    public ICollection<Conversation> Conversations { get; private set; } = new List<Conversation>();
    public ICollection<Review> Reviews { get; private set; } = new List<Review>();
    public ICollection<Complaint> Complaints { get; private set; } = new List<Complaint>();
    public ICollection<ServiceExecution> ServiceExecutions { get; private set; } = new List<ServiceExecution>();

    private Business()
    {
    }

    public Business(long ownerUserId, string name, string? address = null, decimal? lat = null, decimal? lng = null, string? description = null, string? mobile = null)
    {
        OwnerUserId = ownerUserId;
        Name = name;
        Address = address;
        Lat = lat;
        Lng = lng;
        Description = description;
        Mobile = mobile;
        Rating = 0;
        ReviewCount = 0;
        CompletedJobCount = 0;
        Status = "Active";
    }
}
""")

write(ENT / "BusinessProvider.cs", HDR_ENT + """
public class BusinessProvider : AuditableEntity
{
    public long BusinessId { get; private set; }
    public long ProviderId { get; private set; }
    public string Role { get; private set; } = null!;
    public string Status { get; private set; } = "Active";
    public DateTime JoinedAt { get; private set; }
    public DateTime? LeftAt { get; private set; }

    public Business Business { get; private set; } = null!;
    public Provider Provider { get; private set; } = null!;

    private BusinessProvider()
    {
    }

    public BusinessProvider(long businessId, long providerId, string role)
    {
        BusinessId = businessId;
        ProviderId = providerId;
        Role = role;
        Status = "Active";
        JoinedAt = DateTime.UtcNow;
    }

    public void Accept()
    {
        Status = "Active";
        SetUpdated();
    }
}
""")

print("core identity/business written")
print("ok")

using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

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

    public BusinessProvider(long businessId, long providerId, string role = "Member")
    {
        BusinessId = businessId;
        ProviderId = providerId;
        Role = role;
        Status = "Active";
        JoinedAt = DateTime.UtcNow;
    }

    public void UpdateMembership(string role, string status)
    {
        Role = role;
        Status = status;
        if (string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase))
            LeftAt = null;
        SetUpdated();
    }

    public void Leave()
    {
        Status = "Inactive";
        LeftAt = DateTime.UtcNow;
        SoftDelete();
    }

    public void Reactivate(string role)
    {
        Restore();
        Role = role;
        Status = "Active";
        LeftAt = null;
        JoinedAt = DateTime.UtcNow;
    }
}

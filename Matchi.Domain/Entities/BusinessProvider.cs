using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class BusinessProvider : Entity
{
    public long BusinessId { get; private set; }
    public Business Business { get; private set; } = null!;

    public long ProviderId { get; private set; }
    public Provider Provider { get; private set; } = null!;

    public string Role { get; private set; } = "Member";
    public string Status { get; private set; } = "Pending";
    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LeftAt { get; private set; }

    private BusinessProvider() { }

    public BusinessProvider(long businessId, long providerId, string role = "Member")
    {
        BusinessId = businessId;
        ProviderId = providerId;
        Role = role;
        Status = "Pending";
        JoinedAt = DateTime.UtcNow;
    }
}
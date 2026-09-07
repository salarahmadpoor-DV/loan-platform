using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Verification : TimestampedEntity
{
    public string EntityType { get; private set; } = null!;

    public long EntityId { get; private set; };

    public string VerificationType { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public string? Provider { get; private set; };

    public string? Reference { get; private set; };

    public DateTime? VerifiedAt { get; private set; };

    public DateTime? ExpireAt { get; private set; };

    public string? RejectReason { get; private set; };

    public ICollection<VerificationDocument> Documents { get; private set; } = new List<VerificationDocument>();

    private Verification()
    {
    }

    public Verification(string entityType, long entityId, string verificationType, string status)
    {
        EntityType = entityType;
        EntityId = entityId;
        VerificationType = verificationType;
        Status = status;
    }
}

using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class TrustScore : Entity
{
    public string EntityType { get; private set; } = null!;

    public long EntityId { get; private set; };

    public decimal Score { get; private set; };

    public DateTime CalculatedAt { get; private set; };

    private TrustScore()
    {
    }

    public TrustScore(string entityType, long entityId, decimal score)
    {
        EntityType = entityType;
        EntityId = entityId;
        Score = score;
    }
}

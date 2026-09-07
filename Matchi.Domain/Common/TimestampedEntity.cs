namespace Matchi.Domain.Common;

public abstract class TimestampedEntity : Entity
{
    public DateTime CreateDate { get; protected set; }

    public DateTime? UpdateDate { get; protected set; }

    protected TimestampedEntity()
    {
        CreateDate = DateTime.UtcNow;
    }

    protected void SetUpdated()
    {
        UpdateDate = DateTime.UtcNow;
    }
}

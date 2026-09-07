namespace Matchi.Domain.Common;

public abstract class AuditableEntity : TimestampedEntity
{
    public bool IsDeleted { get; protected set; }

    protected AuditableEntity()
    {
        IsDeleted = false;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        SetUpdated();
    }
}

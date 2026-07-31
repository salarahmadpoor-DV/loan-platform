namespace Loan.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime CreateDate { get; protected set; }

    public DateTime? UpdateDate { get; protected set; }

    public bool IsDeleted { get; protected set; }

    protected AuditableEntity()
    {
        CreateDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    protected void SetUpdated()
    {
        UpdateDate = DateTime.UtcNow;
    }
}
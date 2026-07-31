public abstract class Entity
{
    public long Id { get; protected set; }

    public DateTime CreateDate { get; protected set; }

    public DateTime? UpdateDate { get; protected set; }

    public bool IsDeleted { get; protected set; }
}
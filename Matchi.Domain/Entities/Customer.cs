using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Customer : AuditableEntity
{
    public long UserId { get; private set; }

    public User User { get; private set; } = null!;

    public ICollection<Complaint> Complaints { get; private set; } = new List<Complaint>();

    public ICollection<Conversation> Conversations { get; private set; } = new List<Conversation>();

    public ICollection<Deal> Deals { get; private set; } = new List<Deal>();

    public ICollection<Request> Requests { get; private set; } = new List<Request>();

    public ICollection<Review> Reviews { get; private set; } = new List<Review>();

    private Customer()
    {
    }

    public Customer(long userId)
    {
        UserId = userId;
    }
}

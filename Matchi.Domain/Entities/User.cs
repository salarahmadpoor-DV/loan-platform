using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class User : AuditableEntity
{
    public string Mobile { get; private set; } = null!;

    public string? Name { get; private set; }

    public bool IsMobileVerified { get; private set; } = false;

    public Customer? Customer { get; private set; }

    public Provider? Provider { get; private set; }

    public ICollection<Business> Businesses { get; private set; } = new List<Business>();

    public ICollection<Cancellation> Cancellations { get; private set; } = new List<Cancellation>();

    public ICollection<ConversationParticipant> ConversationParticipants { get; private set; } = new List<ConversationParticipant>();

    public ICollection<Message> SentMessages { get; private set; } = new List<Message>();

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

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

using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Bank : AuditableEntity
{
    public string Title { get; private set; } = null!;

    public string Slug { get; private set; } = null!;

    public string? ShortDescription { get; private set; }

    public string? Description { get; private set; }

    public string? TelegramLink { get; private set; }

    public string? EitaaLink { get; private set; }

    public string? RubikaLink { get; private set; }

    public string? SeoTitle { get; private set; }

    public string? SeoDescription { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; } = true;


    public ICollection<BankQuestion> Questions { get; private set; }
        = new List<BankQuestion>();

    public ICollection<LoanRequest> LoanRequests { get; private set; }
        = new List<LoanRequest>();


    private Bank()
    {
    }

    public Bank(string title, string slug)
    {
        Title = title;
        Slug = slug;
        IsActive = true;
    }
}
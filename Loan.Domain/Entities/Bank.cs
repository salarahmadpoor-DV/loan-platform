using Loan.Domain.Common;

namespace Loan.Domain.Entities;

public class Bank : AuditableEntity
{
    public string Title { get; private set; } = null!;

    public string Slug { get; private set; } = null!;

    private Bank()
    {
    }

    public Bank(string title, string slug)
    {
        Title = title;
        Slug = slug;
    }
}
namespace Loan.Application.Features.Banks.Queries.GetBanks;

public sealed class BankDto
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public string? TelegramLink { get; set; }

    public string? EitaaLink { get; set; }

    public string? RubikaLink { get; set; }

    public string? SeoTitle { get; set; }

    public string? SeoDescription { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
}
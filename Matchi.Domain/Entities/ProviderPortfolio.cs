using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ProviderPortfolio : Entity
{
    public long ProviderId { get; private set; }

    public string Title { get; private set; } = null!;

    public string? Description { get; private set; }

    public DateTime CreateDate { get; private set; }

    public Provider Provider { get; private set; } = null!;

    public ICollection<ProviderPortfolioMedia> MediaItems { get; private set; } = new List<ProviderPortfolioMedia>();

    private ProviderPortfolio()
    {
    }

    public ProviderPortfolio(long providerId, string title)
    {
        ProviderId = providerId;
        Title = title;
    }
}

using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class BusinessPortfolio : Entity
{
    public long BusinessId { get; private set; };

    public string Title { get; private set; } = null!;

    public string? Description { get; private set; };

    public DateTime CreateDate { get; private set; };

    public Business Business { get; private set; } = null!;

    public ICollection<BusinessPortfolioMedia> MediaItems { get; private set; } = new List<BusinessPortfolioMedia>();

    private BusinessPortfolio()
    {
    }

    public BusinessPortfolio(long businessId, string title)
    {
        BusinessId = businessId;
        Title = title;
    }
}

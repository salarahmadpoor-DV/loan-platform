using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class BusinessPortfolioMedia : Entity
{
    public long PortfolioId { get; private set; }

    public long MediaId { get; private set; }

    public int DisplayOrder { get; private set; } = 0;

    public Media Media { get; private set; } = null!;

    public BusinessPortfolio Portfolio { get; private set; } = null!;

    private BusinessPortfolioMedia()
    {
    }

    public BusinessPortfolioMedia(long portfolioId, long mediaId)
    {
        PortfolioId = portfolioId;
        MediaId = mediaId;
    }
}

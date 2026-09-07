using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Media : Entity
{
    public string FileName { get; private set; } = null!;

    public string StorageKey { get; private set; } = null!;

    public string ContentType { get; private set; } = null!;

    public long Size { get; private set; }

    public string? Url { get; private set; }

    public DateTime CreateDate { get; private set; }

    public ICollection<Business> LogoBusinesses { get; private set; } = new List<Business>();

    public ICollection<BusinessPortfolioMedia> BusinessPortfolioMedia { get; private set; } = new List<BusinessPortfolioMedia>();

    public ICollection<ProviderPortfolioMedia> ProviderPortfolioMedia { get; private set; } = new List<ProviderPortfolioMedia>();

    public ICollection<VerificationDocument> VerificationDocuments { get; private set; } = new List<VerificationDocument>();

    private Media()
    {
    }

    public Media(string fileName, string storageKey, string contentType, long size)
    {
        FileName = fileName;
        StorageKey = storageKey;
        ContentType = contentType;
        Size = size;
    }
}

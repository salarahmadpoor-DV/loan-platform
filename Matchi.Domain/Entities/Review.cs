using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Review : AuditableEntity
{
    public long DealId { get; private set; }

    public long CustomerId { get; private set; }

    public long? BusinessId { get; private set; }

    public long? ProviderId { get; private set; }

    public byte Rating { get; private set; }

    public string? Comment { get; private set; }

    public Business? Business { get; private set; }

    public Customer Customer { get; private set; } = null!;

    public Deal Deal { get; private set; } = null!;

    public Provider? Provider { get; private set; }

    private Review()
    {
    }

    private Review(
        long dealId,
        long customerId,
        byte rating,
        string? comment,
        long? businessId,
        long? providerId)
    {
        if (dealId <= 0)
            throw new InvalidOperationException("DealId must be greater than zero.");
        if (customerId <= 0)
            throw new InvalidOperationException("CustomerId must be greater than zero.");
        if (rating is < 1 or > 5)
            throw new InvalidOperationException("Rating must be between 1 and 5.");
        if ((businessId is null) == (providerId is null))
            throw new InvalidOperationException("A review must target exactly one of Business or Provider.");
        if (businessId is <= 0)
            throw new InvalidOperationException("BusinessId must be greater than zero when supplied.");
        if (providerId is <= 0)
            throw new InvalidOperationException("ProviderId must be greater than zero when supplied.");

        var trimmed = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        if (trimmed is { Length: > 2000 })
            throw new InvalidOperationException("Comment must be at most 2000 characters.");

        DealId = dealId;
        CustomerId = customerId;
        Rating = rating;
        Comment = trimmed;
        BusinessId = businessId;
        ProviderId = providerId;
    }

    public static Review Create(
        long dealId,
        long customerId,
        byte rating,
        string? comment,
        long? businessId,
        long? providerId)
    {
        return new Review(dealId, customerId, rating, comment, businessId, providerId);
    }
}

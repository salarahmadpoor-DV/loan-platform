using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IReviewRepository
{
    Task<Deal?> GetCustomerDealGraphAsync(
        long dealId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsActiveTargetAsync(
        long dealId,
        long customerId,
        long? businessId,
        long? providerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Review>> ListByProviderAsync(
        long providerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Review>> ListByBusinessAsync(
        long businessId,
        CancellationToken cancellationToken = default);

    void Add(Review review);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IDealRepository
{
    void Add(Deal deal);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Deal>> ListOwnedByUserAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<Deal?> GetOwnedByIdAsync(
        long dealId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Deal>> ListVisibleToProviderAsync(
        long providerId,
        CancellationToken cancellationToken = default);

    Task<bool> HasActiveDealForRequestAsync(
        long requestId,
        CancellationToken cancellationToken = default);
}

using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IProposalRepository
{
    Task AddAsync(Proposal proposal, CancellationToken cancellationToken = default);

    Task UpdateAsync(CancellationToken cancellationToken = default);

    Task<bool> IsRequestOwnedByUserAsync(
        long requestId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<Proposal?> GetTrackedOwnedByRequestOwnerAsync(
        long proposalId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Proposal>> ListOwnedRequestProposalsAsync(
        long requestId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<Proposal?> GetOwnedDetailAsync(
        long proposalId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Proposal>> ListByProviderIdAsync(
        long providerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<long>> GetActiveServiceIdsAsync(
        IReadOnlyCollection<long> serviceIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<long>> GetActiveProductIdsAsync(
        IReadOnlyCollection<long> productIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<long>> GetOfferedServiceIdsAsync(
        bool asProvider,
        long partyId,
        IReadOnlyCollection<long> serviceIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<long>> GetOfferedProductIdsAsync(
        bool asProvider,
        long partyId,
        IReadOnlyCollection<long> productIds,
        CancellationToken cancellationToken = default);
}

using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Matching;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Tests;

internal sealed class FakeCurrentUser : ICurrentUserService
{
    public FakeCurrentUser(long userId)
    {
        UserId = userId;
        IsAuthenticated = true;
    }

    public bool IsAuthenticated { get; }

    public long? UserId { get; }

    public string? Mobile => null;
}

internal sealed class FakeServiceExecutionRepository : IServiceExecutionRepository
{
    public Deal? DealGraph { get; set; }
    public ServiceExecution? Tracked { get; set; }
    public bool Exists { get; set; }
    public bool DealVisible { get; set; } = true;
    public Exception? SaveException { get; set; }
    public List<ServiceExecution> Added { get; } = [];
    public List<ServiceExecution> VisibleToProvider { get; } = [];
    public int SaveCount { get; private set; }

    public Task<Deal?> GetActiveDealGraphAsync(long dealId, CancellationToken cancellationToken = default) =>
        Task.FromResult(DealGraph is not null && DealGraph.Id == dealId ? DealGraph : null);

    public Task<bool> ExistsForDealAsync(long dealId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Exists);

    public Task<bool> IsDealVisibleAsync(long dealId, long userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(DealVisible);

    public void Add(ServiceExecution execution)
    {
        execution.WithId(Added.Count + 1);
        Added.Add(execution);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (SaveException is not null)
            throw SaveException;
        SaveCount++;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ServiceExecution>> ListVisibleByDealAsync(
        long dealId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<ServiceExecution>>([]);

    public Task<IReadOnlyList<ServiceExecution>> ListVisibleToProviderAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<ServiceExecution>>(
            VisibleToProvider.Where(execution =>
                ProviderMarketplaceVisibility.CanSeeExecution(execution, providerId)).ToList());

    public Task<ServiceExecution?> GetVisibleByIdAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Tracked is not null && Tracked.Id == executionId ? Tracked : null);

    public Task<ServiceExecution?> GetTrackedForPartyAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Tracked);

    public Task<ServiceExecution?> GetTrackedForStartCompleteAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Tracked);

    public Task<ServiceExecution?> GetTrackedWithAssignmentsForPartyAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Tracked);
}

internal sealed class FakeExecutionAssignmentRepository : IExecutionAssignmentRepository
{
    public bool HasPrimary { get; set; }
    public bool HasAssignedProvider { get; set; }
    public bool HasMembership { get; set; } = true;
    public bool ProviderExists { get; set; } = true;
    public Exception? SaveException { get; set; }
    public ExecutionAssignment? Tracked { get; set; }
    public List<ExecutionAssignment> Added { get; } = [];
    public int SaveCount { get; private set; }

    public Task<bool> HasPrimaryAsync(long executionId, CancellationToken cancellationToken = default) =>
        Task.FromResult(HasPrimary);

    public Task<bool> HasAssignedProviderAsync(
        long executionId,
        long providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(HasAssignedProvider);

    public Task<bool> HasActiveMembershipAsync(
        long businessId,
        long providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(HasMembership);

    public Task<bool> ProviderExistsAsync(long providerId, CancellationToken cancellationToken = default) =>
        Task.FromResult(ProviderExists);

    public void Add(ExecutionAssignment assignment)
    {
        assignment.WithId(Added.Count + 1);
        Added.Add(assignment);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (SaveException is not null)
            throw SaveException;
        SaveCount++;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ExecutionAssignment>> ListVisibleAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<ExecutionAssignment>>([]);

    public Task<ExecutionAssignment?> GetTrackedForBusinessOwnerAsync(
        long executionId,
        long assignmentId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Tracked);
}

internal sealed class FakeReviewRepository : IReviewRepository
{
    public Deal? Deal { get; set; }
    public bool Exists { get; set; }
    public Exception? SaveException { get; set; }
    public List<Review> Added { get; } = [];
    public List<Review> ProviderReviews { get; } = [];
    public List<Review> BusinessReviews { get; } = [];

    public Task<Deal?> GetCustomerDealGraphAsync(
        long dealId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(
            Deal is not null
            && Deal.Id == dealId
            && Deal.Request.Customer.UserId == userId
                ? Deal
                : null);

    public Task<bool> ExistsActiveTargetAsync(
        long dealId,
        long customerId,
        long? businessId,
        long? providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Exists);

    public Task<IReadOnlyList<Review>> ListByProviderAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Review>>(
            ProviderReviews.Where(r => r.ProviderId == providerId && !r.IsDeleted).ToList());

    public Task<IReadOnlyList<Review>> ListByBusinessAsync(
        long businessId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Review>>(
            BusinessReviews.Where(r => r.BusinessId == businessId && !r.IsDeleted).ToList());

    public void Add(Review review)
    {
        review.WithId(Added.Count + 1);
        Added.Add(review);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (SaveException is not null)
            throw SaveException;
        return Task.CompletedTask;
    }
}

internal sealed class FakeDealRepository : IDealRepository
{
    public bool HasActiveDeal { get; set; }
    public bool ThrowIfActiveDealQueried { get; set; }
    public int ActiveDealQueryCount { get; private set; }
    public List<Deal> Added { get; } = [];
    public List<Deal> VisibleToProvider { get; } = [];
    public int SaveCount { get; private set; }

    public void Add(Deal deal)
    {
        deal.WithId(Added.Count + 1);
        Added.Add(deal);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Deal>> ListOwnedByUserAsync(
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Deal>>([]);

    public Task<Deal?> GetOwnedByIdAsync(
        long dealId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<Deal?>(null);

    public Task<IReadOnlyList<Deal>> ListVisibleToProviderAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Deal>>(
            VisibleToProvider.Where(deal => ProviderMarketplaceVisibility.CanSeeDeal(deal, providerId)).ToList());

    public Task<bool> HasActiveDealForRequestAsync(
        long requestId,
        CancellationToken cancellationToken = default)
    {
        if (ThrowIfActiveDealQueried)
            throw new InvalidOperationException("Active deal state must not be queried before ownership is established.");

        ActiveDealQueryCount++;
        return Task.FromResult(HasActiveDeal);
    }
}

internal sealed class FakeRequestRepository : IRequestRepository
{
    public Request? Owned { get; set; }
    public int UpdateCount { get; private set; }

    public Task<Customer> GetOrCreateCustomerAsync(long userId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> ServiceExistsAsync(long serviceId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<ServiceAttribute?> GetServiceAttributeAsync(
        long serviceId,
        long serviceAttributeId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Product?> GetProductAsync(long productId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> ProductCategoryExistsAsync(long productCategoryId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<ProductAttribute?> GetProductAttributeAsync(
        long productCategoryId,
        long productAttributeId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddAsync(Request request, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task UpdateAsync(Request request, CancellationToken cancellationToken = default)
    {
        UpdateCount++;
        return Task.CompletedTask;
    }

    public void RemoveLocationsAndSchedules(Request request) => throw new NotSupportedException();

    public Task<Request?> GetByIdAsync(
        long requestId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Request?> GetOwnedByIdAsync(
        long requestId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Owned is not null && Owned.Id == requestId ? Owned : null);

    public Task<IReadOnlyList<Request>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}

internal sealed class FakeProposalRepository : IProposalRepository
{
    public Proposal? Tracked { get; set; }
    public Func<long, Proposal?>? ResolveTracked { get; set; }
    public List<Proposal> ByProvider { get; } = [];

    public Task AddAsync(Proposal proposal, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task UpdateAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<bool> IsRequestOwnedByUserAsync(
        long requestId,
        long userId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Proposal?> GetTrackedOwnedByRequestOwnerAsync(
        long proposalId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        if (ResolveTracked is not null)
            return Task.FromResult(ResolveTracked(proposalId));

        return Task.FromResult(
            Tracked is not null && Tracked.Id == proposalId ? Tracked : null);
    }

    public Task<IReadOnlyList<Proposal>> ListOwnedRequestProposalsAsync(
        long requestId,
        long userId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Proposal?> GetOwnedDetailAsync(
        long proposalId,
        long userId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<Proposal>> ListByProviderIdAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Proposal>>(
            ByProvider.Where(proposal => proposal.ProviderId == providerId && !proposal.IsDeleted).ToList());

    public Task<IReadOnlyList<long>> GetActiveServiceIdsAsync(
        IReadOnlyCollection<long> serviceIds,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<long>> GetActiveProductIdsAsync(
        IReadOnlyCollection<long> productIds,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<long>> GetOfferedServiceIdsAsync(
        bool asProvider,
        long partyId,
        IReadOnlyCollection<long> serviceIds,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<long>> GetOfferedProductIdsAsync(
        bool asProvider,
        long partyId,
        IReadOnlyCollection<long> productIds,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}

internal sealed class FakeMatchingReadRepository : IMatchingReadRepository
{
    public long EligibleForProviderId { get; set; } = MarketplaceGraph.ProviderEntityId;
    public List<Request> Eligible { get; } = [];

    public Task<IReadOnlyList<MatchingCandidateRow>> FindProviderMatchesAsync(
        MatchingCriteria criteria,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<MatchingCandidateRow>> FindBusinessMatchesAsync(
        MatchingCriteria criteria,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<Request>> ListOpenEligibleRequestsForProviderAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Request>>(
            providerId == EligibleForProviderId ? Eligible : []);
}

internal sealed class FakeProviderRepository : IProviderRepository
{
    public Provider? Mine { get; set; }

    public Task<Provider?> GetByIdAsync(long providerId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Provider?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Mine is not null && Mine.UserId == userId ? Mine : null);

    public Task AddAsync(Provider provider, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task UpdateAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IEnumerable<Provider>> GetProvidersByServiceIdAsync(
        long serviceId,
        int skip,
        int take,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> ServiceExistsActiveAsync(long serviceId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> ProductExistsActiveAsync(long productId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<ServiceAttribute?> GetServiceAttributeAsync(
        long serviceAttributeId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> OffersServiceAsync(long providerId, long serviceId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<ProviderService?> GetServiceLinkAsync(
        long providerId,
        long serviceId,
        bool includeDeleted,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<ProviderService>> ListServicesAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddServiceAsync(ProviderService link, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<ProviderProduct?> GetProductLinkAsync(
        long providerId,
        long productId,
        bool includeDeleted,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<ProviderProduct>> ListProductsAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddProductAsync(ProviderProduct link, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<ProviderCapability?> GetCapabilityAsync(
        long providerId,
        long serviceAttributeId,
        bool includeDeleted,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<ProviderCapability>> ListCapabilitiesAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddCapabilityAsync(ProviderCapability capability, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<ProviderServiceArea?> GetServiceAreaAsync(
        long providerId,
        long areaId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<ProviderServiceArea>> ListServiceAreasAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddServiceAreaAsync(ProviderServiceArea area, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<ProviderAvailability?> GetAvailabilityAsync(
        long providerId,
        long availabilityId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<ProviderAvailability>> ListAvailabilitiesAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddAvailabilityAsync(ProviderAvailability availability, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> HasAvailabilityOverlapAsync(
        long providerId,
        byte dayOfWeek,
        TimeSpan timeFrom,
        TimeSpan timeTo,
        long? excludeId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<BusinessProvider>> ListMembershipsAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}

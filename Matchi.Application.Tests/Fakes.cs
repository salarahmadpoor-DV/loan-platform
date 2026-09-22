using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Matching;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Matchi.Domain.Locations;

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

    public Task<bool> ServiceExistsAsync(long serviceId, CancellationToken cancellationToken = default) =>
        Task.FromResult(ExistingServiceIds.Contains(serviceId));

    public HashSet<long> ExistingServiceIds { get; } = [];

    public Task<ServiceAttribute?> GetServiceAttributeAsync(
        long serviceId,
        long serviceAttributeId,
        CancellationToken cancellationToken = default)
    {
        if (!ServiceAttributes.TryGetValue((serviceId, serviceAttributeId), out var attribute))
            return Task.FromResult<ServiceAttribute?>(null);

        if (attribute.IsDeleted || !attribute.IsActive)
            return Task.FromResult<ServiceAttribute?>(null);

        return Task.FromResult<ServiceAttribute?>(attribute);
    }

    public Dictionary<(long ServiceId, long AttributeId), ServiceAttribute> ServiceAttributes { get; } = [];

    public Dictionary<long, Product> ProductsById { get; } = [];

    public Task<Product?> GetProductAsync(long productId, CancellationToken cancellationToken = default)
    {
        if (!ProductsById.TryGetValue(productId, out var product))
            return Task.FromResult<Product?>(null);

        if (product.IsDeleted || !product.IsActive)
            return Task.FromResult<Product?>(null);

        return Task.FromResult<Product?>(product);
    }

    public HashSet<long> ExistingCategoryIds { get; } = [];

    public Task<bool> ProductCategoryExistsAsync(long productCategoryId, CancellationToken cancellationToken = default) =>
        Task.FromResult(ExistingCategoryIds.Contains(productCategoryId));

    public Dictionary<(long CategoryId, long AttributeId), ProductAttribute> ProductAttributes { get; } = [];

    public Task<ProductAttribute?> GetProductAttributeAsync(
        long productCategoryId,
        long productAttributeId,
        CancellationToken cancellationToken = default)
    {
        if (!ProductAttributes.TryGetValue((productCategoryId, productAttributeId), out var attribute))
            return Task.FromResult<ProductAttribute?>(null);

        if (attribute.IsDeleted || !attribute.IsActive)
            return Task.FromResult<ProductAttribute?>(null);

        return Task.FromResult<ProductAttribute?>(attribute);
    }

    public Customer Customer { get; set; } = new Customer(1).WithId(1);

    public List<Request> Added { get; } = [];

    public Task<Customer> GetOrCreateCustomerAsync(long userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Customer);

    public Task AddAsync(Request request, CancellationToken cancellationToken = default)
    {
        if (request.Id == 0)
            request.WithId(Added.Count + 1);

        Added.Add(request);
        return Task.CompletedTask;
    }

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

internal sealed class FakeLocationReadRepository : ILocationReadRepository
{
    public List<LocationProvince> Provinces { get; } = [];
    public List<LocationCity> Cities { get; } = [];
    public List<LocationDistrict> Districts { get; } = [];

    public Task<IReadOnlyList<LocationProvince>> GetActiveProvincesAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<LocationProvince>>(Provinces.Where(x => x.IsActive).ToList());

        public Task<IReadOnlyList<LocationCity>> GetActiveCitiesByProvinceIdAsync(
        long provinceId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<LocationCity>>(
            Cities.Where(x => x.IsActive && x.ProvinceId == provinceId).ToList());

    public Task<IReadOnlyList<LocationCity>> GetActiveCitiesAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<LocationCity>>(Cities.Where(x => x.IsActive).ToList());

    public Task<IReadOnlyList<LocationDistrict>> GetActiveDistrictsByCityIdAsync(
        long cityId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<LocationDistrict>>(
            Districts.Where(x => x.IsActive && x.CityId == cityId).ToList());

    public Task<LocationProvince?> FindProvinceByIdAsync(
        long provinceId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Provinces.FirstOrDefault(x => x.Id == provinceId));

    public Task<LocationCity?> FindCityByIdAsync(
        long cityId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Cities.FirstOrDefault(x => x.Id == cityId));

    public Task<LocationDistrict?> FindDistrictByIdAsync(
        long districtId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Districts.FirstOrDefault(x => x.Id == districtId));

    public Task<IReadOnlyList<LocationCity>> GetActiveCoveredCitiesAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<LocationCity>>(
            Cities.Where(x =>
                x.IsActive
                && x.CenterLat != null
                && x.CenterLng != null
                && x.RadiusKm != null).ToList());

    public Task<IReadOnlyList<LocationDistrict>> GetActiveCoveredDistrictsByCityIdAsync(
        long cityId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<LocationDistrict>>(
            Districts.Where(x =>
                x.IsActive
                && x.CityId == cityId
                && x.CenterLat != null
                && x.CenterLng != null
                && x.RadiusKm != null).ToList());
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
    public List<MatchingCandidateRow> ProviderMatches { get; } = [];
    public List<MatchingCandidateRow> BusinessMatches { get; } = [];
    public bool ThrowIfMatchQueried { get; set; }
    public int ProviderMatchQueryCount { get; private set; }

    public Task<IReadOnlyList<MatchingCandidateRow>> FindProviderMatchesAsync(
        MatchingCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        if (ThrowIfMatchQueried)
            throw new InvalidOperationException("Matching must not run for a cancelled request.");

        ProviderMatchQueryCount++;
        return Task.FromResult<IReadOnlyList<MatchingCandidateRow>>(ProviderMatches);
    }

    public Task<IReadOnlyList<MatchingCandidateRow>> FindBusinessMatchesAsync(
        MatchingCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        if (ThrowIfMatchQueried)
            throw new InvalidOperationException("Matching must not run for a cancelled request.");

        return Task.FromResult<IReadOnlyList<MatchingCandidateRow>>(BusinessMatches);
    }

    public Task<IReadOnlyList<Request>> ListOpenEligibleRequestsForProviderAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Request>>(
            providerId == EligibleForProviderId ? Eligible : []);
}

internal sealed class FakeProviderRepository : IProviderRepository
{
    public Provider? Mine { get; set; }
    public HashSet<long> ActiveCatalogServiceIds { get; } = [];
    public HashSet<long> ActiveCatalogProductIds { get; } = [];
    public List<ProviderService> ServiceLinks { get; } = [];
    public List<ProviderProduct> ProductLinks { get; } = [];
    public int UpdateCount { get; private set; }

    public Task<Provider?> GetByIdAsync(long providerId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Provider?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Mine is not null && Mine.UserId == userId ? Mine : null);

    public Task AddAsync(Provider provider, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        UpdateCount++;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Provider>> GetProvidersByServiceIdAsync(
        long serviceId,
        int skip,
        int take,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> ServiceExistsActiveAsync(long serviceId, CancellationToken cancellationToken = default) =>
        Task.FromResult(ActiveCatalogServiceIds.Contains(serviceId));

    public Task<bool> ProductExistsActiveAsync(long productId, CancellationToken cancellationToken = default) =>
        Task.FromResult(ActiveCatalogProductIds.Contains(productId));

    public Task<ServiceAttribute?> GetServiceAttributeAsync(
        long serviceAttributeId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> OffersServiceAsync(long providerId, long serviceId, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceLinks.Any(x =>
            x.ProviderId == providerId && x.ServiceId == serviceId && !x.IsDeleted && x.IsActive));

    public Task<ProviderService?> GetServiceLinkAsync(
        long providerId,
        long serviceId,
        bool includeDeleted,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceLinks.FirstOrDefault(x =>
            x.ProviderId == providerId
            && x.ServiceId == serviceId
            && (includeDeleted || !x.IsDeleted)));

    public Task<IReadOnlyList<ProviderService>> ListServicesAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<ProviderService>>(
            ServiceLinks.Where(x => x.ProviderId == providerId && !x.IsDeleted).ToList());

    public Task AddServiceAsync(ProviderService link, CancellationToken cancellationToken = default)
    {
        if (link.Id == 0)
            link.WithId(ServiceLinks.Count + 1);
        ServiceLinks.Add(link);
        return Task.CompletedTask;
    }

    public Task<ProviderProduct?> GetProductLinkAsync(
        long providerId,
        long productId,
        bool includeDeleted,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(ProductLinks.FirstOrDefault(x =>
            x.ProviderId == providerId
            && x.ProductId == productId
            && (includeDeleted || !x.IsDeleted)));

    public Task<IReadOnlyList<ProviderProduct>> ListProductsAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<ProviderProduct>>(
            ProductLinks.Where(x => x.ProviderId == providerId && !x.IsDeleted).ToList());

    public Task AddProductAsync(ProviderProduct link, CancellationToken cancellationToken = default)
    {
        if (link.Id == 0)
            link.WithId(ProductLinks.Count + 1);
        ProductLinks.Add(link);
        return Task.CompletedTask;
    }

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
        Task.FromResult<IReadOnlyList<BusinessProvider>>([]);
}

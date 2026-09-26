using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Businesses.Commands;
using Matchi.Application.Features.Matching;
using Matchi.Application.Features.Providers.Commands;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Tests;

public sealed class OfferingCatalogTests
{
    [Fact]
    public async Task Provider_can_add_active_service()
    {
        var providers = ProviderRepo();
        providers.ActiveCatalogServiceIds.Add(10);
        var id = await new AddProviderServiceCommandHandler(new FakeCurrentUser(5), providers)
            .Handle(new AddProviderServiceCommand(10), CancellationToken.None);

        Assert.True(id > 0);
        Assert.Single(providers.ServiceLinks);
        Assert.Equal(10, providers.ServiceLinks[0].ServiceId);
        Assert.True(providers.ServiceLinks[0].IsActive);
        Assert.Empty(await providers.ListMembershipsAsync(providers.Mine!.Id));
    }

    [Fact]
    public async Task Provider_cannot_add_inactive_or_missing_service()
    {
        var providers = ProviderRepo();
        var handler = new AddProviderServiceCommandHandler(new FakeCurrentUser(5), providers);

        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new AddProviderServiceCommand(10), CancellationToken.None));
        Assert.Contains(ex.Errors, e => e.ErrorMessage.Contains("not found or is inactive", StringComparison.Ordinal));
        Assert.Empty(providers.ServiceLinks);
    }

    [Fact]
    public async Task Provider_cannot_add_deleted_service()
    {
        var providers = ProviderRepo();
        var handler = new AddProviderServiceCommandHandler(new FakeCurrentUser(5), providers);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new AddProviderServiceCommand(99), CancellationToken.None));
    }

    [Fact]
    public async Task Duplicate_active_provider_service_is_rejected()
    {
        var providers = ProviderRepo();
        providers.ActiveCatalogServiceIds.Add(10);
        var handler = new AddProviderServiceCommandHandler(new FakeCurrentUser(5), providers);
        await handler.Handle(new AddProviderServiceCommand(10), CancellationToken.None);

        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new AddProviderServiceCommand(10), CancellationToken.None));
        Assert.Contains(ex.Errors, e => e.ErrorMessage.Contains("already linked", StringComparison.Ordinal));
        Assert.Single(providers.ServiceLinks);
    }

    [Fact]
    public async Task Provider_can_add_product_and_rejects_inactive_or_deleted()
    {
        var providers = ProviderRepo();
        providers.ActiveCatalogProductIds.Add(20);
        var added = await new AddProviderProductCommandHandler(new FakeCurrentUser(5), providers)
            .Handle(new AddProviderProductCommand(20, 150000m), CancellationToken.None);
        Assert.True(added > 0);
        Assert.Equal(150000m, providers.ProductLinks[0].Price);

        var missing = await Assert.ThrowsAsync<ValidationException>(() =>
            new AddProviderProductCommandHandler(new FakeCurrentUser(5), providers)
                .Handle(new AddProviderProductCommand(21, null), CancellationToken.None));
        Assert.Contains(missing.Errors, e => e.ErrorMessage.Contains("not found or is inactive", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Business_owner_can_add_service_and_product()
    {
        var businesses = BusinessRepo();
        businesses.ActiveCatalogServiceIds.Add(10);
        businesses.ActiveCatalogProductIds.Add(20);

        var serviceId = await new AddBusinessServiceCommandHandler(new FakeCurrentUser(9), businesses)
            .Handle(new AddBusinessServiceCommand(7, 10, MinPrice: 100, MaxPrice: 200), CancellationToken.None);
        var productId = await new AddBusinessProductCommandHandler(new FakeCurrentUser(9), businesses)
            .Handle(new AddBusinessProductCommand(7, 20, 50000m), CancellationToken.None);

        Assert.True(serviceId > 0);
        Assert.True(productId > 0);
        Assert.Equal(100m, businesses.ServiceLinks[0].MinPrice);
        Assert.Equal(50000m, businesses.ProductLinks[0].Price);
    }

    [Fact]
    public async Task Non_owner_cannot_modify_business_offerings()
    {
        var businesses = BusinessRepo();
        businesses.ActiveCatalogServiceIds.Add(10);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new AddBusinessServiceCommandHandler(new FakeCurrentUser(99), businesses)
                .Handle(new AddBusinessServiceCommand(7, 10), CancellationToken.None));
    }

    [Fact]
    public async Task Business_rejects_inactive_service_and_duplicate_product()
    {
        var businesses = BusinessRepo();
        businesses.ActiveCatalogProductIds.Add(20);

        await Assert.ThrowsAsync<ValidationException>(() =>
            new AddBusinessServiceCommandHandler(new FakeCurrentUser(9), businesses)
                .Handle(new AddBusinessServiceCommand(7, 10), CancellationToken.None));

        var handler = new AddBusinessProductCommandHandler(new FakeCurrentUser(9), businesses);
        await handler.Handle(new AddBusinessProductCommand(7, 20, null), CancellationToken.None);
        var duplicate = await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new AddBusinessProductCommand(7, 20, null), CancellationToken.None));
        Assert.Contains(duplicate.Errors, e => e.ErrorMessage.Contains("already linked", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Provider_exists_without_business_membership()
    {
        var providers = ProviderRepo();
        Assert.Empty(await providers.ListMembershipsAsync(providers.Mine!.Id));
        Assert.NotNull(providers.Mine);
    }

    private static FakeProviderRepository ProviderRepo()
    {
        return new FakeProviderRepository
        {
            Mine = new Provider(5, "مستقل", "09120000000").WithId(3)
        };
    }

    private static OfferingBusinessRepository BusinessRepo()
    {
        var repo = new OfferingBusinessRepository();
        repo.Owned.Add(new Business(9, "کسب‌وکار نمونه").WithId(7));
        return repo;
    }
}

public sealed class MatchingOfferEligibilityTests
{
    [Fact]
    public void Provider_offering_requested_service_is_eligible()
    {
        Assert.True(MatchingOfferEligibility.ServiceLinkMatches(false, true, 10, [10, 11]));
        Assert.True(MatchingOfferEligibility.CandidateQualifies(true, true, false, false));
    }

    [Fact]
    public void Provider_not_offering_requested_service_is_not_eligible_when_required()
    {
        Assert.False(MatchingOfferEligibility.ServiceLinkMatches(false, true, 12, [10]));
        Assert.False(MatchingOfferEligibility.CandidateQualifies(requireService: true, serviceMatch: false, requireProduct: false, productMatch: false));
    }

    [Fact]
    public void Business_service_and_product_eligibility_mirrors_provider_links()
    {
        Assert.True(MatchingOfferEligibility.ServiceLinkMatches(false, true, 10, [10]));
        Assert.False(MatchingOfferEligibility.ServiceLinkMatches(false, false, 10, [10]));
        Assert.True(MatchingOfferEligibility.ProductLinkMatches(false, true, 20, 3, false, [20], []));
        Assert.True(MatchingOfferEligibility.ProductLinkMatches(false, true, 99, 3, false, [], [3]));
        Assert.False(MatchingOfferEligibility.ProductLinkMatches(false, false, 20, 3, false, [20], []));
        Assert.False(MatchingOfferEligibility.ProductLinkMatches(false, true, 20, 3, true, [20], [3]));
        Assert.True(MatchingOfferEligibility.CandidateQualifies(false, false, true, true));
        Assert.False(MatchingOfferEligibility.CandidateQualifies(false, false, true, false));
    }

    [Fact]
    public void Inactive_or_deleted_links_are_ignored()
    {
        Assert.False(MatchingOfferEligibility.ServiceLinkMatches(true, true, 10, [10]));
        Assert.False(MatchingOfferEligibility.ServiceLinkMatches(false, false, 10, [10]));
        Assert.False(MatchingOfferEligibility.ProductLinkMatches(true, true, 20, 3, false, [20], []));
        Assert.False(MatchingOfferEligibility.ProductLinkMatches(false, false, 20, 3, false, [20], []));
    }

    [Fact]
    public void Any_requested_line_is_enough_all_lines_are_not_required()
    {
        Assert.True(MatchingOfferEligibility.ServiceLinkMatches(false, true, 10, [10, 11, 12]));
        Assert.False(MatchingOfferEligibility.ServiceLinkMatches(false, true, 99, [10, 11]));
    }

    [Fact]
    public void Hybrid_request_requires_service_not_product()
    {
        var request = new Request(1, "Hybrid", "نصب و قطعه").WithId(1);
        request.AddService(new RequestService(1, 10));
        request.AddProduct(new RequestProduct(1, 20, 3));
        var criteria = GetRequestMatchesQueryHandler.ToCriteria(request);

        Assert.True(criteria.RequireService);
        Assert.False(criteria.RequireProduct);
        Assert.Contains(10, criteria.ServiceIds);
        Assert.Contains(20, criteria.ProductIds);
        Assert.True(MatchingOfferEligibility.CandidateQualifies(
            criteria.RequireService, serviceMatch: true, criteria.RequireProduct, productMatch: false));
    }

    [Fact]
    public void Independent_provider_row_is_a_distinct_candidate_type()
    {
        var provider = new MatchingCandidateRow(MatchCandidateType.Provider, 3, "مستقل", 50);
        var business = new MatchingCandidateRow(MatchCandidateType.Business, 7, "کسب‌وکار", 50);
        Assert.Equal(MatchCandidateType.Provider, provider.CandidateType);
        Assert.Equal(MatchCandidateType.Business, business.CandidateType);
        Assert.NotEqual(provider.CandidateType, business.CandidateType);
    }
}

internal sealed class OfferingBusinessRepository : IBusinessRepository
{
    public List<Business> Owned { get; } = [];
    public HashSet<long> ActiveCatalogServiceIds { get; } = [];
    public HashSet<long> ActiveCatalogProductIds { get; } = [];
    public List<BusinessService> ServiceLinks { get; } = [];
    public List<BusinessProduct> ProductLinks { get; } = [];

    public Task AddAsync(Business business, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task UpdateAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<Business?> GetByIdAsync(long businessId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<Business>> GetByOwnerUserIdAsync(
        long ownerUserId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Business>>(Owned.Where(b => b.OwnerUserId == ownerUserId).ToList());

    public Task<IReadOnlyList<Business>> ListAsync(int skip, int take, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> ServiceExistsActiveAsync(long serviceId, CancellationToken cancellationToken = default) =>
        Task.FromResult(ActiveCatalogServiceIds.Contains(serviceId));

    public Task<bool> ProductExistsActiveAsync(long productId, CancellationToken cancellationToken = default) =>
        Task.FromResult(ActiveCatalogProductIds.Contains(productId));

    public Task<bool> ProviderExistsAsync(long providerId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<BusinessService?> GetServiceLinkAsync(
        long businessId,
        long serviceId,
        bool includeDeleted,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceLinks.FirstOrDefault(x =>
            x.BusinessId == businessId && x.ServiceId == serviceId && (includeDeleted || !x.IsDeleted)));

    public Task<IReadOnlyList<BusinessService>> ListServicesAsync(
        long businessId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<BusinessService>>(
            ServiceLinks.Where(x => x.BusinessId == businessId && !x.IsDeleted).ToList());

    public Task AddServiceAsync(BusinessService link, CancellationToken cancellationToken = default)
    {
        if (link.Id == 0)
            link.WithId(ServiceLinks.Count + 1);
        ServiceLinks.Add(link);
        return Task.CompletedTask;
    }

    public Task<BusinessProduct?> GetProductLinkAsync(
        long businessId,
        long productId,
        bool includeDeleted,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(ProductLinks.FirstOrDefault(x =>
            x.BusinessId == businessId && x.ProductId == productId && (includeDeleted || !x.IsDeleted)));

    public Task<IReadOnlyList<BusinessProduct>> ListProductsAsync(
        long businessId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<BusinessProduct>>(
            ProductLinks.Where(x => x.BusinessId == businessId && !x.IsDeleted).ToList());

    public Task AddProductAsync(BusinessProduct link, CancellationToken cancellationToken = default)
    {
        if (link.Id == 0)
            link.WithId(ProductLinks.Count + 1);
        ProductLinks.Add(link);
        return Task.CompletedTask;
    }

    public Task<BusinessServiceArea?> GetServiceAreaAsync(long businessId, long areaId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<BusinessServiceArea>> ListServiceAreasAsync(long businessId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddServiceAreaAsync(BusinessServiceArea area, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<BusinessAvailability?> GetAvailabilityAsync(long businessId, long availabilityId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<BusinessAvailability>> ListAvailabilitiesAsync(long businessId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddAvailabilityAsync(BusinessAvailability availability, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> HasAvailabilityOverlapAsync(
        long businessId, byte dayOfWeek, TimeSpan timeFrom, TimeSpan timeTo, long? excludeId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<BusinessProvider?> GetMembershipAsync(long businessId, long providerId, bool includeDeleted, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<BusinessProvider>> ListMembershipsAsync(long businessId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<BusinessProvider>>([]);

    public Task AddMembershipAsync(BusinessProvider membership, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<BusinessProvider?> GetMembershipByIdAsync(long membershipId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}

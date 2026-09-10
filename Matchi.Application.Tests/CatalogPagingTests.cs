using Matchi.Application.Common;
using Matchi.Application.Features.Businesses.Queries.GetBusinesses;
using Matchi.Application.Features.Providers.Queries;
using Matchi.Application.Features.Services.Queries;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Tests;

public sealed class ListPagingTests
{
    [Fact]
    public void Normalize_ClampsPageAndPageSize()
    {
        Assert.Equal((1, 20, 0), ListPaging.Normalize(0, 0));
        Assert.Equal((1, 20, 0), ListPaging.Normalize(-3, -1));
        Assert.Equal((2, 100, 100), ListPaging.Normalize(2, 500));
        Assert.Equal((3, 10, 20), ListPaging.Normalize(3, 10));
    }
}

public sealed class CatalogPagingHandlerTests
{
    [Fact]
    public async Task GetServices_AppliesQueryPagingToRepository()
    {
        var services = new RecordingServiceRepository
        {
            Items =
            {
                new Service("Alpha", 1).WithId(1),
                new Service("Beta", 1).WithId(2)
            }
        };
        var handler = new GetServicesQueryHandler(services);

        var result = (await handler.Handle(
            new GetServicesQuery(9, "Al", 2, 5),
            CancellationToken.None)).ToList();

        Assert.Equal(9, services.CategoryId);
        Assert.Equal("Al", services.Query);
        Assert.Equal(5, services.Skip);
        Assert.Equal(5, services.Take);
        Assert.Equal(2, result.Count);
        Assert.Equal("Alpha", result[0].Name);
    }

    [Fact]
    public async Task GetBusinesses_AppliesSkipTake()
    {
        var businesses = new RecordingBusinessListRepository();
        var handler = new GetBusinessesQueryHandler(businesses);

        await handler.Handle(new GetBusinessesQuery(2, 10), CancellationToken.None);

        Assert.Equal(10, businesses.Skip);
        Assert.Equal(10, businesses.Take);
    }

    [Fact]
    public async Task SearchProviders_WithoutServiceId_DoesNotQuery()
    {
        var providers = new RecordingProviderSearchRepository();
        var handler = new SearchProvidersQueryHandler(providers);

        var result = await handler.Handle(
            new SearchProvidersQuery(null, null, null, 10, null, 1, 20),
            CancellationToken.None);

        Assert.Empty(result);
        Assert.False(providers.Queried);
    }

    [Fact]
    public async Task SearchProviders_WithServiceId_AppliesPaging()
    {
        var providers = new RecordingProviderSearchRepository
        {
            Items = { new Provider(10, "P", "09120000000").WithId(4) }
        };
        var handler = new SearchProvidersQueryHandler(providers);

        var result = (await handler.Handle(
            new SearchProvidersQuery(3, null, null, 10, null, 2, 7),
            CancellationToken.None)).ToList();

        Assert.True(providers.Queried);
        Assert.Equal(3, providers.ServiceId);
        Assert.Equal(7, providers.Skip);
        Assert.Equal(7, providers.Take);
        Assert.Single(result);
        Assert.Equal(4, result[0].Id);
    }
}

internal sealed class RecordingServiceRepository : IServiceRepository
{
    public long? CategoryId { get; private set; }
    public string? Query { get; private set; }
    public int Skip { get; private set; }
    public int Take { get; private set; }
    public List<Service> Items { get; } = [];

    public Task<IEnumerable<ServiceCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IEnumerable<Service>> GetServicesAsync(
        long? categoryId,
        string? query,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        CategoryId = categoryId;
        Query = query;
        Skip = skip;
        Take = take;
        return Task.FromResult<IEnumerable<Service>>(Items);
    }

    public Task<Service?> GetServiceByIdAsync(long serviceId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}

internal sealed class RecordingBusinessListRepository : IBusinessRepository
{
    public int Skip { get; private set; }
    public int Take { get; private set; }

    public Task AddAsync(Business business, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task UpdateAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Business?> GetByIdAsync(long businessId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<Business>> GetByOwnerUserIdAsync(
        long ownerUserId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<Business>> ListAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        Skip = skip;
        Take = take;
        return Task.FromResult<IReadOnlyList<Business>>([]);
    }

    public Task<bool> ServiceExistsActiveAsync(long serviceId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> ProductExistsActiveAsync(long productId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> ProviderExistsAsync(long providerId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<BusinessService?> GetServiceLinkAsync(
        long businessId,
        long serviceId,
        bool includeDeleted,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<BusinessService>> ListServicesAsync(
        long businessId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddServiceAsync(BusinessService link, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<BusinessProduct?> GetProductLinkAsync(
        long businessId,
        long productId,
        bool includeDeleted,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<BusinessProduct>> ListProductsAsync(
        long businessId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddProductAsync(BusinessProduct link, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<BusinessServiceArea?> GetServiceAreaAsync(
        long businessId,
        long areaId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<BusinessServiceArea>> ListServiceAreasAsync(
        long businessId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddServiceAreaAsync(BusinessServiceArea area, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<BusinessAvailability?> GetAvailabilityAsync(
        long businessId,
        long availabilityId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<BusinessAvailability>> ListAvailabilitiesAsync(
        long businessId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddAvailabilityAsync(BusinessAvailability slot, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<bool> HasAvailabilityOverlapAsync(
        long businessId,
        byte dayOfWeek,
        TimeSpan timeFrom,
        TimeSpan timeTo,
        long? excludeId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<BusinessProvider?> GetMembershipAsync(
        long businessId,
        long providerId,
        bool includeDeleted,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<BusinessProvider>> ListMembershipsAsync(
        long businessId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddMembershipAsync(BusinessProvider membership, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<BusinessProvider?> GetMembershipByIdAsync(
        long membershipId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}

internal sealed class RecordingProviderSearchRepository : IProviderRepository
{
    public bool Queried { get; private set; }
    public long ServiceId { get; private set; }
    public int Skip { get; private set; }
    public int Take { get; private set; }
    public List<Provider> Items { get; } = [];

    public Task<Provider?> GetByIdAsync(long providerId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Provider?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddAsync(Provider provider, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task UpdateAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IEnumerable<Provider>> GetProvidersByServiceIdAsync(
        long serviceId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        Queried = true;
        ServiceId = serviceId;
        Skip = skip;
        Take = take;
        return Task.FromResult<IEnumerable<Provider>>(Items);
    }

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

using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IProviderRepository
{
    Task<Provider?> GetByIdAsync(long providerId, CancellationToken cancellationToken = default);

    Task<Provider?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    Task AddAsync(Provider provider, CancellationToken cancellationToken = default);

    Task UpdateAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Provider>> GetProvidersByServiceIdAsync(
        long serviceId,
        CancellationToken cancellationToken = default);

    Task<bool> ServiceExistsActiveAsync(long serviceId, CancellationToken cancellationToken = default);

    Task<bool> ProductExistsActiveAsync(long productId, CancellationToken cancellationToken = default);

    Task<ServiceAttribute?> GetServiceAttributeAsync(
        long serviceAttributeId,
        CancellationToken cancellationToken = default);

    Task<bool> OffersServiceAsync(long providerId, long serviceId, CancellationToken cancellationToken = default);

    Task<ProviderService?> GetServiceLinkAsync(
        long providerId,
        long serviceId,
        bool includeDeleted,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProviderService>> ListServicesAsync(
        long providerId,
        CancellationToken cancellationToken = default);

    Task AddServiceAsync(ProviderService link, CancellationToken cancellationToken = default);

    Task<ProviderProduct?> GetProductLinkAsync(
        long providerId,
        long productId,
        bool includeDeleted,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProviderProduct>> ListProductsAsync(
        long providerId,
        CancellationToken cancellationToken = default);

    Task AddProductAsync(ProviderProduct link, CancellationToken cancellationToken = default);

    Task<ProviderCapability?> GetCapabilityAsync(
        long providerId,
        long serviceAttributeId,
        bool includeDeleted,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProviderCapability>> ListCapabilitiesAsync(
        long providerId,
        CancellationToken cancellationToken = default);

    Task AddCapabilityAsync(ProviderCapability capability, CancellationToken cancellationToken = default);

    Task<ProviderServiceArea?> GetServiceAreaAsync(
        long providerId,
        long areaId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProviderServiceArea>> ListServiceAreasAsync(
        long providerId,
        CancellationToken cancellationToken = default);

    Task AddServiceAreaAsync(ProviderServiceArea area, CancellationToken cancellationToken = default);

    Task<ProviderAvailability?> GetAvailabilityAsync(
        long providerId,
        long availabilityId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProviderAvailability>> ListAvailabilitiesAsync(
        long providerId,
        CancellationToken cancellationToken = default);

    Task AddAvailabilityAsync(ProviderAvailability availability, CancellationToken cancellationToken = default);

    Task<bool> HasAvailabilityOverlapAsync(
        long providerId,
        byte dayOfWeek,
        TimeSpan timeFrom,
        TimeSpan timeTo,
        long? excludeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BusinessProvider>> ListMembershipsAsync(
        long providerId,
        CancellationToken cancellationToken = default);
}

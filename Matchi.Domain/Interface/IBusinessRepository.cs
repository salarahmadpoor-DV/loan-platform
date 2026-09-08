using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IBusinessRepository
{
    Task AddAsync(Business business, CancellationToken cancellationToken = default);

    Task UpdateAsync(CancellationToken cancellationToken = default);

    Task<Business?> GetByIdAsync(long businessId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Business>> GetByOwnerUserIdAsync(
        long ownerUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Business>> ListAsync(CancellationToken cancellationToken = default);

    Task<bool> ServiceExistsActiveAsync(long serviceId, CancellationToken cancellationToken = default);

    Task<bool> ProductExistsActiveAsync(long productId, CancellationToken cancellationToken = default);

    Task<bool> ProviderExistsAsync(long providerId, CancellationToken cancellationToken = default);

    Task<BusinessService?> GetServiceLinkAsync(
        long businessId,
        long serviceId,
        bool includeDeleted,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BusinessService>> ListServicesAsync(
        long businessId,
        CancellationToken cancellationToken = default);

    Task AddServiceAsync(BusinessService link, CancellationToken cancellationToken = default);

    Task<BusinessProduct?> GetProductLinkAsync(
        long businessId,
        long productId,
        bool includeDeleted,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BusinessProduct>> ListProductsAsync(
        long businessId,
        CancellationToken cancellationToken = default);

    Task AddProductAsync(BusinessProduct link, CancellationToken cancellationToken = default);

    Task<BusinessServiceArea?> GetServiceAreaAsync(
        long businessId,
        long areaId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BusinessServiceArea>> ListServiceAreasAsync(
        long businessId,
        CancellationToken cancellationToken = default);

    Task AddServiceAreaAsync(BusinessServiceArea area, CancellationToken cancellationToken = default);

    Task<BusinessAvailability?> GetAvailabilityAsync(
        long businessId,
        long availabilityId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BusinessAvailability>> ListAvailabilitiesAsync(
        long businessId,
        CancellationToken cancellationToken = default);

    Task AddAvailabilityAsync(BusinessAvailability availability, CancellationToken cancellationToken = default);

    Task<bool> HasAvailabilityOverlapAsync(
        long businessId,
        byte dayOfWeek,
        TimeSpan timeFrom,
        TimeSpan timeTo,
        long? excludeId,
        CancellationToken cancellationToken = default);

    Task<BusinessProvider?> GetMembershipAsync(
        long businessId,
        long providerId,
        bool includeDeleted,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BusinessProvider>> ListMembershipsAsync(
        long businessId,
        CancellationToken cancellationToken = default);

    Task AddMembershipAsync(BusinessProvider membership, CancellationToken cancellationToken = default);

    Task<BusinessProvider?> GetMembershipByIdAsync(long membershipId, CancellationToken cancellationToken = default);
}

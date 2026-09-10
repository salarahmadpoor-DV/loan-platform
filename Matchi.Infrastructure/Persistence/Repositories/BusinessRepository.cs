using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class BusinessRepository : IBusinessRepository
{
    private readonly MatchiDbContext _context;

    public BusinessRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Business business, CancellationToken cancellationToken = default)
    {
        await _context.Businesses.AddAsync(business, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task<Business?> GetByIdAsync(long businessId, CancellationToken cancellationToken = default)
    {
        return _context.Businesses
            .FirstOrDefaultAsync(x => x.Id == businessId && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Business>> GetByOwnerUserIdAsync(
        long ownerUserId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Businesses
            .Where(x => x.OwnerUserId == ownerUserId && !x.IsDeleted)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Business>> ListAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await _context.Businesses
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ServiceExistsActiveAsync(long serviceId, CancellationToken cancellationToken = default)
    {
        return _context.Services.AnyAsync(
            s => s.Id == serviceId && !s.IsDeleted && s.IsActive,
            cancellationToken);
    }

    public Task<bool> ProductExistsActiveAsync(long productId, CancellationToken cancellationToken = default)
    {
        return _context.Products.AnyAsync(
            p => p.Id == productId && !p.IsDeleted && p.IsActive,
            cancellationToken);
    }

    public Task<bool> ProviderExistsAsync(long providerId, CancellationToken cancellationToken = default)
    {
        return _context.Providers.AnyAsync(
            p => p.Id == providerId && !p.IsDeleted,
            cancellationToken);
    }

    public Task<BusinessService?> GetServiceLinkAsync(
        long businessId,
        long serviceId,
        bool includeDeleted,
        CancellationToken cancellationToken = default)
    {
        return _context.BusinessServices
            .FirstOrDefaultAsync(
                x => x.BusinessId == businessId
                     && x.ServiceId == serviceId
                     && (includeDeleted || !x.IsDeleted),
                cancellationToken);
    }

    public async Task<IReadOnlyList<BusinessService>> ListServicesAsync(
        long businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.BusinessServices
            .AsNoTracking()
            .Include(x => x.Service)
            .Where(x => x.BusinessId == businessId && !x.IsDeleted)
            .OrderBy(x => x.ServiceId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddServiceAsync(BusinessService link, CancellationToken cancellationToken = default)
    {
        await _context.BusinessServices.AddAsync(link, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<BusinessProduct?> GetProductLinkAsync(
        long businessId,
        long productId,
        bool includeDeleted,
        CancellationToken cancellationToken = default)
    {
        return _context.BusinessProducts
            .FirstOrDefaultAsync(
                x => x.BusinessId == businessId
                     && x.ProductId == productId
                     && (includeDeleted || !x.IsDeleted),
                cancellationToken);
    }

    public async Task<IReadOnlyList<BusinessProduct>> ListProductsAsync(
        long businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.BusinessProducts
            .AsNoTracking()
            .Include(x => x.Product)
            .Where(x => x.BusinessId == businessId && !x.IsDeleted)
            .OrderBy(x => x.ProductId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddProductAsync(BusinessProduct link, CancellationToken cancellationToken = default)
    {
        await _context.BusinessProducts.AddAsync(link, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<BusinessServiceArea?> GetServiceAreaAsync(
        long businessId,
        long areaId,
        CancellationToken cancellationToken = default)
    {
        return _context.BusinessServiceAreas
            .FirstOrDefaultAsync(
                x => x.Id == areaId && x.BusinessId == businessId && !x.IsDeleted,
                cancellationToken);
    }

    public async Task<IReadOnlyList<BusinessServiceArea>> ListServiceAreasAsync(
        long businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.BusinessServiceAreas
            .AsNoTracking()
            .Where(x => x.BusinessId == businessId && !x.IsDeleted)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddServiceAreaAsync(BusinessServiceArea area, CancellationToken cancellationToken = default)
    {
        await _context.BusinessServiceAreas.AddAsync(area, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<BusinessAvailability?> GetAvailabilityAsync(
        long businessId,
        long availabilityId,
        CancellationToken cancellationToken = default)
    {
        return _context.BusinessAvailabilities
            .FirstOrDefaultAsync(
                x => x.Id == availabilityId && x.BusinessId == businessId && !x.IsDeleted,
                cancellationToken);
    }

    public async Task<IReadOnlyList<BusinessAvailability>> ListAvailabilitiesAsync(
        long businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.BusinessAvailabilities
            .AsNoTracking()
            .Where(x => x.BusinessId == businessId && !x.IsDeleted)
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.TimeFrom)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAvailabilityAsync(BusinessAvailability availability, CancellationToken cancellationToken = default)
    {
        await _context.BusinessAvailabilities.AddAsync(availability, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> HasAvailabilityOverlapAsync(
        long businessId,
        byte dayOfWeek,
        TimeSpan timeFrom,
        TimeSpan timeTo,
        long? excludeId,
        CancellationToken cancellationToken = default)
    {
        return _context.BusinessAvailabilities.AnyAsync(
            x => x.BusinessId == businessId
                 && !x.IsDeleted
                 && x.IsAvailable
                 && x.DayOfWeek == dayOfWeek
                 && (!excludeId.HasValue || x.Id != excludeId.Value)
                 && x.TimeFrom < timeTo
                 && timeFrom < x.TimeTo,
            cancellationToken);
    }

    public Task<BusinessProvider?> GetMembershipAsync(
        long businessId,
        long providerId,
        bool includeDeleted,
        CancellationToken cancellationToken = default)
    {
        return _context.BusinessProviders
            .FirstOrDefaultAsync(
                x => x.BusinessId == businessId
                     && x.ProviderId == providerId
                     && (includeDeleted || !x.IsDeleted),
                cancellationToken);
    }

    public async Task<IReadOnlyList<BusinessProvider>> ListMembershipsAsync(
        long businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.BusinessProviders
            .AsNoTracking()
            .Include(x => x.Provider)
            .Where(x => x.BusinessId == businessId && !x.IsDeleted)
            .OrderBy(x => x.ProviderId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddMembershipAsync(BusinessProvider membership, CancellationToken cancellationToken = default)
    {
        await _context.BusinessProviders.AddAsync(membership, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<BusinessProvider?> GetMembershipByIdAsync(
        long membershipId,
        CancellationToken cancellationToken = default)
    {
        return _context.BusinessProviders
            .Include(x => x.Provider)
            .FirstOrDefaultAsync(x => x.Id == membershipId && !x.IsDeleted, cancellationToken);
    }
}

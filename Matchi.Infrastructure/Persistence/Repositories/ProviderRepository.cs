using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public class ProviderRepository : IProviderRepository
{
    private readonly MatchiDbContext _context;

    public ProviderRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public Task<Provider?> GetByIdAsync(long providerId, CancellationToken cancellationToken = default)
    {
        return _context.Providers
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken);
    }

    public Task<Provider?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return _context.Providers
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Provider provider, CancellationToken cancellationToken = default)
    {
        await _context.Providers.AddAsync(provider, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Provider>> GetProvidersByServiceIdAsync(
        long serviceId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProviderServices
            .AsNoTracking()
            .Where(ps => ps.ServiceId == serviceId && !ps.IsDeleted && ps.IsActive)
            .Join(
                _context.Providers,
                ps => ps.ProviderId,
                p => p.Id,
                (ps, p) => p)
            .Where(p => p.Status == "Active" && !p.IsDeleted)
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

    public Task<ServiceAttribute?> GetServiceAttributeAsync(
        long serviceAttributeId,
        CancellationToken cancellationToken = default)
    {
        return _context.ServiceAttributes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                a => a.Id == serviceAttributeId && !a.IsDeleted && a.IsActive,
                cancellationToken);
    }

    public Task<bool> OffersServiceAsync(long providerId, long serviceId, CancellationToken cancellationToken = default)
    {
        return _context.ProviderServices.AnyAsync(
            x => x.ProviderId == providerId
                 && x.ServiceId == serviceId
                 && !x.IsDeleted
                 && x.IsActive,
            cancellationToken);
    }

    public Task<ProviderService?> GetServiceLinkAsync(
        long providerId,
        long serviceId,
        bool includeDeleted,
        CancellationToken cancellationToken = default)
    {
        return _context.ProviderServices
            .FirstOrDefaultAsync(
                x => x.ProviderId == providerId
                     && x.ServiceId == serviceId
                     && (includeDeleted || !x.IsDeleted),
                cancellationToken);
    }

    public async Task<IReadOnlyList<ProviderService>> ListServicesAsync(
        long providerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProviderServices
            .AsNoTracking()
            .Include(x => x.Service)
            .Where(x => x.ProviderId == providerId && !x.IsDeleted)
            .OrderBy(x => x.ServiceId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddServiceAsync(ProviderService link, CancellationToken cancellationToken = default)
    {
        await _context.ProviderServices.AddAsync(link, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<ProviderProduct?> GetProductLinkAsync(
        long providerId,
        long productId,
        bool includeDeleted,
        CancellationToken cancellationToken = default)
    {
        return _context.ProviderProducts
            .FirstOrDefaultAsync(
                x => x.ProviderId == providerId
                     && x.ProductId == productId
                     && (includeDeleted || !x.IsDeleted),
                cancellationToken);
    }

    public async Task<IReadOnlyList<ProviderProduct>> ListProductsAsync(
        long providerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProviderProducts
            .AsNoTracking()
            .Include(x => x.Product)
            .Where(x => x.ProviderId == providerId && !x.IsDeleted)
            .OrderBy(x => x.ProductId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddProductAsync(ProviderProduct link, CancellationToken cancellationToken = default)
    {
        await _context.ProviderProducts.AddAsync(link, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<ProviderCapability?> GetCapabilityAsync(
        long providerId,
        long serviceAttributeId,
        bool includeDeleted,
        CancellationToken cancellationToken = default)
    {
        return _context.ProviderCapabilities
            .FirstOrDefaultAsync(
                x => x.ProviderId == providerId
                     && x.ServiceAttributeId == serviceAttributeId
                     && (includeDeleted || !x.IsDeleted),
                cancellationToken);
    }

    public async Task<IReadOnlyList<ProviderCapability>> ListCapabilitiesAsync(
        long providerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProviderCapabilities
            .AsNoTracking()
            .Include(x => x.ServiceAttribute)
            .Where(x => x.ProviderId == providerId && !x.IsDeleted)
            .OrderBy(x => x.ServiceAttributeId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddCapabilityAsync(ProviderCapability capability, CancellationToken cancellationToken = default)
    {
        await _context.ProviderCapabilities.AddAsync(capability, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<ProviderServiceArea?> GetServiceAreaAsync(
        long providerId,
        long areaId,
        CancellationToken cancellationToken = default)
    {
        return _context.ProviderServiceAreas
            .FirstOrDefaultAsync(
                x => x.Id == areaId && x.ProviderId == providerId && !x.IsDeleted,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ProviderServiceArea>> ListServiceAreasAsync(
        long providerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProviderServiceAreas
            .AsNoTracking()
            .Where(x => x.ProviderId == providerId && !x.IsDeleted)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddServiceAreaAsync(ProviderServiceArea area, CancellationToken cancellationToken = default)
    {
        await _context.ProviderServiceAreas.AddAsync(area, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<ProviderAvailability?> GetAvailabilityAsync(
        long providerId,
        long availabilityId,
        CancellationToken cancellationToken = default)
    {
        return _context.ProviderAvailabilities
            .FirstOrDefaultAsync(
                x => x.Id == availabilityId && x.ProviderId == providerId && !x.IsDeleted,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ProviderAvailability>> ListAvailabilitiesAsync(
        long providerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProviderAvailabilities
            .AsNoTracking()
            .Where(x => x.ProviderId == providerId && !x.IsDeleted)
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.TimeFrom)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAvailabilityAsync(ProviderAvailability availability, CancellationToken cancellationToken = default)
    {
        await _context.ProviderAvailabilities.AddAsync(availability, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> HasAvailabilityOverlapAsync(
        long providerId,
        byte dayOfWeek,
        TimeSpan timeFrom,
        TimeSpan timeTo,
        long? excludeId,
        CancellationToken cancellationToken = default)
    {
        return _context.ProviderAvailabilities.AnyAsync(
            x => x.ProviderId == providerId
                 && !x.IsDeleted
                 && x.IsAvailable
                 && x.DayOfWeek == dayOfWeek
                 && (!excludeId.HasValue || x.Id != excludeId.Value)
                 && x.TimeFrom < timeTo
                 && timeFrom < x.TimeTo,
            cancellationToken);
    }

    public async Task<IReadOnlyList<BusinessProvider>> ListMembershipsAsync(
        long providerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.BusinessProviders
            .AsNoTracking()
            .Include(x => x.Business)
            .Where(x => x.ProviderId == providerId && !x.IsDeleted)
            .OrderBy(x => x.BusinessId)
            .ToListAsync(cancellationToken);
    }
}

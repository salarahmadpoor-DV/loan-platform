using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Matching;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class MatchingReadRepository : IMatchingReadRepository
{
    private readonly MatchiDbContext _context;

    public MatchingReadRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<MatchingCandidateRow>> FindProviderMatchesAsync(
        MatchingCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var serviceIds = criteria.ServiceIds.ToList();
        var productIds = criteria.ProductIds.ToList();
        var categoryIds = criteria.ProductCategoryIds.ToList();
        var attributeIds = criteria.ServiceAttributeIds.ToList();
        var city = criteria.City;
        var province = criteria.Province;
        var district = criteria.District;
        var dayOfWeek = criteria.DayOfWeek;
        var timeFrom = criteria.TimeFrom;
        var timeTo = criteria.TimeTo;
        var isFlexible = criteria.IsFlexible;
        var requireService = criteria.RequireService;
        var requireProduct = criteria.RequireProduct;
        var hasLocation = city is not null || province is not null || district is not null;
        var hasSchedule = dayOfWeek.HasValue;
        var hasAttributes = attributeIds.Count > 0;
        var hasProducts = productIds.Count > 0 || categoryIds.Count > 0;

        var query =
            from provider in _context.Providers.AsNoTracking()
            where !provider.IsDeleted && provider.Status == "Active"
            let serviceMatch = serviceIds.Count != 0 && _context.ProviderServices.Any(link =>
                link.ProviderId == provider.Id
                && !link.IsDeleted
                && link.IsActive
                && serviceIds.Contains(link.ServiceId))
            let productMatch = hasProducts && _context.ProviderProducts.Any(link =>
                link.ProviderId == provider.Id
                && !link.IsDeleted
                && link.IsAvailable
                && (productIds.Contains(link.ProductId)
                    || _context.Products.Any(product =>
                        product.Id == link.ProductId
                        && !product.IsDeleted
                        && categoryIds.Contains(product.CategoryId))))
            where (!requireService || serviceMatch) && (!requireProduct || productMatch)
            let capabilityMatch = hasAttributes && _context.ProviderCapabilities.Any(capability =>
                capability.ProviderId == provider.Id
                && !capability.IsDeleted
                && attributeIds.Contains(capability.ServiceAttributeId))
            let areaMatch = hasLocation && _context.ProviderServiceAreas.Any(area =>
                area.ProviderId == provider.Id
                && !area.IsDeleted
                && area.IsActive
                && ((city != null && area.City != null && area.City.ToLower() == city)
                    || (province != null && area.Province != null && area.Province.ToLower() == province)
                    || (district != null && area.District != null && area.District.ToLower() == district)))
            let availabilityMatch = hasSchedule && _context.ProviderAvailabilities.Any(slot =>
                slot.ProviderId == provider.Id
                && !slot.IsDeleted
                && slot.IsAvailable
                && slot.DayOfWeek == dayOfWeek
                && (isFlexible
                    || timeFrom == null
                    || timeTo == null
                    || (slot.TimeFrom < timeTo && timeFrom < slot.TimeTo)))
            select new MatchingCandidateRow(
                MatchCandidateType.Provider,
                provider.Id,
                provider.Name,
                (serviceMatch ? MatchingScores.Service : 0)
                + (productMatch ? MatchingScores.Product : 0)
                + (capabilityMatch ? MatchingScores.Capability : 0)
                + (areaMatch ? MatchingScores.Area : 0)
                + (availabilityMatch ? MatchingScores.Availability : 0));

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MatchingCandidateRow>> FindBusinessMatchesAsync(
        MatchingCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var serviceIds = criteria.ServiceIds.ToList();
        var productIds = criteria.ProductIds.ToList();
        var categoryIds = criteria.ProductCategoryIds.ToList();
        var city = criteria.City;
        var province = criteria.Province;
        var district = criteria.District;
        var dayOfWeek = criteria.DayOfWeek;
        var timeFrom = criteria.TimeFrom;
        var timeTo = criteria.TimeTo;
        var isFlexible = criteria.IsFlexible;
        var requireService = criteria.RequireService;
        var requireProduct = criteria.RequireProduct;
        var hasLocation = city is not null || province is not null || district is not null;
        var hasSchedule = dayOfWeek.HasValue;
        var hasProducts = productIds.Count > 0 || categoryIds.Count > 0;

        var query =
            from business in _context.Businesses.AsNoTracking()
            where !business.IsDeleted && business.Status == "Active"
            let serviceMatch = serviceIds.Count != 0 && _context.BusinessServices.Any(link =>
                link.BusinessId == business.Id
                && !link.IsDeleted
                && link.IsActive
                && serviceIds.Contains(link.ServiceId))
            let productMatch = hasProducts && _context.BusinessProducts.Any(link =>
                link.BusinessId == business.Id
                && !link.IsDeleted
                && link.IsAvailable
                && (productIds.Contains(link.ProductId)
                    || _context.Products.Any(product =>
                        product.Id == link.ProductId
                        && !product.IsDeleted
                        && categoryIds.Contains(product.CategoryId))))
            where (!requireService || serviceMatch) && (!requireProduct || productMatch)
            let areaMatch = hasLocation && _context.BusinessServiceAreas.Any(area =>
                area.BusinessId == business.Id
                && !area.IsDeleted
                && area.IsActive
                && ((city != null && area.City != null && area.City.ToLower() == city)
                    || (province != null && area.Province != null && area.Province.ToLower() == province)
                    || (district != null && area.District != null && area.District.ToLower() == district)))
            let availabilityMatch = hasSchedule && _context.BusinessAvailabilities.Any(slot =>
                slot.BusinessId == business.Id
                && !slot.IsDeleted
                && slot.IsAvailable
                && slot.DayOfWeek == dayOfWeek
                && (isFlexible
                    || timeFrom == null
                    || timeTo == null
                    || (slot.TimeFrom < timeTo && timeFrom < slot.TimeTo)))
            select new MatchingCandidateRow(
                MatchCandidateType.Business,
                business.Id,
                business.Name,
                (serviceMatch ? MatchingScores.Service : 0)
                + (productMatch ? MatchingScores.Product : 0)
                + (areaMatch ? MatchingScores.Area : 0)
                + (availabilityMatch ? MatchingScores.Availability : 0));

        return await query.ToListAsync(cancellationToken);
    }
}

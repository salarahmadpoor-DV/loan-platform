using Matchi.Domain.Interfaces;
using Matchi.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class LocationReadRepository : ILocationReadRepository
{
    private readonly MatchiDbContext _context;

    public LocationReadRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<LocationProvince>> GetActiveProvincesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.LocationProvinces
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LocationCity>> GetActiveCitiesByProvinceIdAsync(
        long provinceId,
        CancellationToken cancellationToken = default)
    {
        return await _context.LocationCities
            .AsNoTracking()
            .Where(x => x.IsActive && x.ProvinceId == provinceId)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LocationCity>> GetActiveCitiesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.LocationCities
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LocationDistrict>> GetActiveDistrictsByCityIdAsync(
        long cityId,
        CancellationToken cancellationToken = default)
    {
        return await _context.LocationDistricts
            .AsNoTracking()
            .Where(x => x.IsActive && x.CityId == cityId)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<LocationProvince?> FindProvinceByIdAsync(
        long provinceId,
        CancellationToken cancellationToken = default)
    {
        return _context.LocationProvinces
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == provinceId, cancellationToken);
    }

    public Task<LocationCity?> FindCityByIdAsync(
        long cityId,
        CancellationToken cancellationToken = default)
    {
        return _context.LocationCities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == cityId, cancellationToken);
    }

    public Task<LocationDistrict?> FindDistrictByIdAsync(
        long districtId,
        CancellationToken cancellationToken = default)
    {
        return _context.LocationDistricts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == districtId, cancellationToken);
    }

    public async Task<IReadOnlyList<LocationCity>> GetActiveCoveredCitiesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.LocationCities
            .AsNoTracking()
            .Include(x => x.Province)
            .Where(x =>
                x.IsActive
                && x.Province.IsActive
                && x.CenterLat != null
                && x.CenterLng != null
                && x.RadiusKm != null)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LocationDistrict>> GetActiveCoveredDistrictsByCityIdAsync(
        long cityId,
        CancellationToken cancellationToken = default)
    {
        return await _context.LocationDistricts
            .AsNoTracking()
            .Where(x =>
                x.IsActive
                && x.CityId == cityId
                && x.CenterLat != null
                && x.CenterLng != null
                && x.RadiusKm != null)
            .ToListAsync(cancellationToken);
    }
}

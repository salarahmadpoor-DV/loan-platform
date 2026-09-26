using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Locations;
using Matchi.Application.Features.Matching;
using Matchi.Domain.Interfaces;
using Matchi.Domain.Locations;

namespace Matchi.Infrastructure.Locations;

public sealed class InternalLocationResolver : IInternalLocationResolver
{
    private readonly ILocationReadRepository _locations;

    public InternalLocationResolver(ILocationReadRepository locations)
    {
        _locations = locations;
    }

    public async Task<LocationResolveResult?> ResolveAsync(
        decimal lat,
        decimal lng,
        CancellationToken cancellationToken = default)
    {
        var pointLat = (double)lat;
        var pointLng = (double)lng;
        if (!GeoDistance.IsValidPoint(pointLat, pointLng))
            return null;

        var cities = await _locations.GetActiveCoveredCitiesAsync(cancellationToken);
        var city = cities
            .Select(item => new
            {
                City = item,
                DistanceKm = DistanceKm(pointLat, pointLng, item)
            })
            .Where(item =>
                item.DistanceKm is not null
                && item.City.RadiusKm is not null
                && item.DistanceKm.Value <= (double)item.City.RadiusKm.Value)
            .OrderBy(item => item.DistanceKm)
            .Select(item => item.City)
            .FirstOrDefault();

        if (city is null)
            return null;

        var district = await ResolveDistrictAsync(city.Id, pointLat, pointLng, cancellationToken);

        return new LocationResolveResult(
            city.ProvinceId,
            city.Province.Name,
            city.Id,
            city.Name,
            district?.Id,
            district?.Name,
            LocationResolveSources.Internal);
    }

    private async Task<LocationDistrict?> ResolveDistrictAsync(
        long cityId,
        double lat,
        double lng,
        CancellationToken cancellationToken)
    {
        var covered = await _locations.GetActiveCoveredDistrictsByCityIdAsync(cityId, cancellationToken);
        var match = covered
            .Select(item => new
            {
                District = item,
                DistanceKm = DistanceKm(lat, lng, item.CenterLat, item.CenterLng)
            })
            .Where(item =>
                item.DistanceKm is not null
                && item.District.RadiusKm is not null
                && item.DistanceKm.Value <= (double)item.District.RadiusKm.Value)
            .OrderBy(item => item.DistanceKm)
            .Select(item => item.District)
            .FirstOrDefault();

        if (match is not null)
            return match;

        var districts = await _locations.GetActiveDistrictsByCityIdAsync(cityId, cancellationToken);
        return districts.Count == 1 ? districts[0] : null;
    }

    private static double? DistanceKm(double lat, double lng, LocationCity city) =>
        DistanceKm(lat, lng, city.CenterLat, city.CenterLng);

    private static double? DistanceKm(double lat, double lng, decimal? centerLat, decimal? centerLng)
    {
        if (centerLat is null || centerLng is null)
            return null;

        return GeoDistance.HaversineKm(lat, lng, (double)centerLat.Value, (double)centerLng.Value);
    }
}

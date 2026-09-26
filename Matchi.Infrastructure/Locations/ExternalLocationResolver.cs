using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Locations;
using Matchi.Domain.Interfaces;
using Matchi.Domain.Locations;

namespace Matchi.Infrastructure.Locations;

public sealed class ExternalLocationResolver : IExternalLocationResolver
{
    private readonly IExternalGeocodingClient _client;
    private readonly ILocationReadRepository _locations;

    public ExternalLocationResolver(
        IExternalGeocodingClient client,
        ILocationReadRepository locations)
    {
        _client = client;
        _locations = locations;
    }

    public async Task<LocationResolveResult?> ResolveAsync(
        decimal lat,
        decimal lng,
        CancellationToken cancellationToken = default)
    {
        ExternalGeocodePlace? place;
        try
        {
            place = await _client.ReverseAsync(lat, lng, cancellationToken);
        }
        catch
        {
            return null;
        }

        if (place is null || string.IsNullOrWhiteSpace(place.CityName))
            return null;

        var provinces = await _locations.GetActiveProvincesAsync(cancellationToken);
        var province = Match(provinces, place.ProvinceName, x => x.Name)
            ?? Match(provinces, place.ProvinceName, x => x.Code);

        var cities = province is not null
            ? await _locations.GetActiveCitiesByProvinceIdAsync(province.Id, cancellationToken)
            : await _locations.GetActiveCitiesAsync(cancellationToken);

        var city = Match(cities, place.CityName, x => x.Name);
        if (city is null)
            return null;

        province ??= provinces.FirstOrDefault(x => x.Id == city.ProvinceId);
        if (province is null)
            return null;

        LocationDistrict? district = null;
        if (!string.IsNullOrWhiteSpace(place.DistrictName))
        {
            var districts = await _locations.GetActiveDistrictsByCityIdAsync(city.Id, cancellationToken);
            district = Match(districts, place.DistrictName, x => x.Name);
        }

        return new LocationResolveResult(
            province.Id,
            province.Name,
            city.Id,
            city.Name,
            district?.Id,
            district?.Name,
            LocationResolveSources.External);
    }

    private static T? Match<T>(IEnumerable<T> items, string? raw, Func<T, string> name)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var needle = Normalize(raw);
        return items.FirstOrDefault(item => Normalize(name(item)) == needle)
            ?? items.FirstOrDefault(item =>
            {
                var haystack = Normalize(name(item));
                return haystack.Contains(needle, StringComparison.Ordinal)
                    || needle.Contains(haystack, StringComparison.Ordinal);
            });
    }

    private static string Normalize(string value) => value.Trim().ToLowerInvariant();
}

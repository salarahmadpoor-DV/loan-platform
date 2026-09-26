using System.Net.Http.Headers;
using System.Text.Json;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Locations;

namespace Matchi.Infrastructure.Locations;

/// <summary>
/// Configurable reverse-geocoding HTTP client. Default JSON shape matches Nominatim-style
/// <c>address.state / city / suburb</c> fields. Swap BaseUrl (and optionally ApiKey) per environment.
/// </summary>
public sealed class HttpExternalGeocodingClient : IExternalGeocodingClient
{
    private readonly HttpClient _http;
    private readonly LocationResolverOptions _options;

    public HttpExternalGeocodingClient(LocationResolverOptions options)
    {
        _options = options;
        _http = new HttpClient
        {
            Timeout = TimeSpan.FromMilliseconds(Math.Max(250, options.TimeoutMs + 250))
        };
    }

    public async Task<ExternalGeocodePlace?> ReverseAsync(
        decimal lat,
        decimal lng,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = _options.External.BaseUrl?.Trim();
        if (string.IsNullOrWhiteSpace(baseUrl))
            return null;

        try
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromMilliseconds(Math.Max(1, _options.TimeoutMs)));

            var url = BuildUrl(baseUrl, lat, lng, _options.External.ApiKey);
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrWhiteSpace(_options.External.UserAgent))
                request.Headers.UserAgent.ParseAdd(_options.External.UserAgent);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _http.SendAsync(request, timeout.Token);
            if (!response.IsSuccessStatusCode)
                return null;

            await using var stream = await response.Content.ReadAsStreamAsync(timeout.Token);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: timeout.Token);
            return Parse(document.RootElement);
        }
        catch
        {
            return null;
        }
    }

    private static string BuildUrl(string baseUrl, decimal lat, decimal lng, string? apiKey)
    {
        var separator = baseUrl.Contains('?', StringComparison.Ordinal) ? "&" : "?";
        var url =
            $"{baseUrl.TrimEnd('/')}{separator}lat={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lon={lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}&format=json";
        if (!string.IsNullOrWhiteSpace(apiKey))
            url += $"&key={Uri.EscapeDataString(apiKey)}";
        return url;
    }

    private static ExternalGeocodePlace? Parse(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object)
            return null;

        JsonElement address = root;
        if (root.TryGetProperty("address", out var nested) && nested.ValueKind == JsonValueKind.Object)
            address = nested;

        var province = FirstString(address, "state", "province", "region", "ISO3166-2-lvl4");
        var city = FirstString(address, "city", "town", "village", "municipality", "county");
        var district = FirstString(
            address,
            "suburb",
            "neighbourhood",
            "neighborhood",
            "city_district",
            "district",
            "quarter");

        if (string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(province))
            return null;

        return new ExternalGeocodePlace(province, city, district);
    }

    private static string? FirstString(JsonElement element, params string[] names)
    {
        foreach (var name in names)
        {
            if (element.TryGetProperty(name, out var value)
                && value.ValueKind == JsonValueKind.String)
            {
                var text = value.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                    return text;
            }
        }

        return null;
    }
}

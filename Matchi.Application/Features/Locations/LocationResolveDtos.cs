namespace Matchi.Application.Features.Locations;

public static class LocationResolveSources
{
    public const string External = "External";
    public const string Internal = "Internal";
}

public sealed record LocationResolveResult(
    long ProvinceId,
    string ProvinceName,
    long CityId,
    string CityName,
    long? DistrictId,
    string? DistrictName,
    string Source);

public sealed record LocationResolveApiResponse(bool Success, LocationResolveResult? Data);

public sealed class LocationResolverOptions
{
    public const string SectionName = "LocationResolver";

    public string Mode { get; set; } = "Auto";

    public bool ExternalEnabled { get; set; } = true;

    public int TimeoutMs { get; set; } = 1500;

    public bool FallbackToInternal { get; set; } = true;

    public ExternalGeocodingOptions External { get; set; } = new();
}

public sealed class ExternalGeocodingOptions
{
    public string BaseUrl { get; set; } = "";

    public string UserAgent { get; set; } = "Matchi/1.0 (location-resolver)";

    public string? ApiKey { get; set; }
}

public sealed record ExternalGeocodePlace(
    string? ProvinceName,
    string? CityName,
    string? DistrictName);

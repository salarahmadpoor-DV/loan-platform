using Matchi.Domain.Entities;

namespace Matchi.Application.Features.Providers.Queries;

public sealed class ProviderDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public double Rating { get; init; }
    public bool IsActive { get; init; }
    public double? Lat { get; init; }
    public double? Lng { get; init; }

    public ProviderDto(long id, string name, double rating, bool isActive, double? lat, double? lng)
    {
        Id = id;
        Name = name;
        Rating = rating;
        IsActive = isActive;
        Lat = lat;
        Lng = lng;
    }

    public static ProviderDto From(Provider provider) =>
        new(
            provider.Id,
            provider.Name,
            (double)provider.Rating,
            string.Equals(provider.Status, "Active", StringComparison.OrdinalIgnoreCase),
            provider.Lat.HasValue ? (double)provider.Lat.Value : null,
            provider.Lng.HasValue ? (double)provider.Lng.Value : null);
}

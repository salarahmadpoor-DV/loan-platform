using Matchi.Application.Features.Locations;

namespace Matchi.Application.Common.Interfaces;

public interface ILocationResolver
{
    Task<LocationResolveResult?> ResolveAsync(
        decimal lat,
        decimal lng,
        CancellationToken cancellationToken = default);
}

public interface IExternalLocationResolver : ILocationResolver;

public interface IInternalLocationResolver : ILocationResolver;

public interface IExternalGeocodingClient
{
    Task<ExternalGeocodePlace?> ReverseAsync(
        decimal lat,
        decimal lng,
        CancellationToken cancellationToken = default);
}

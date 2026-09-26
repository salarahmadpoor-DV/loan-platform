using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Locations;
using MediatR;

namespace Matchi.Application.Features.Locations.Queries;

public sealed record ResolveLocationQuery(decimal Lat, decimal Lng)
    : IRequest<LocationResolveApiResponse>;

public sealed class ResolveLocationQueryHandler
    : IRequestHandler<ResolveLocationQuery, LocationResolveApiResponse>
{
    private readonly ILocationResolver _resolver;

    public ResolveLocationQueryHandler(ILocationResolver resolver)
    {
        _resolver = resolver;
    }

    public async Task<LocationResolveApiResponse> Handle(
        ResolveLocationQuery request,
        CancellationToken cancellationToken)
    {
        var lat = (double)request.Lat;
        var lng = (double)request.Lng;
        if (!Matching.GeoDistance.IsValidPoint(lat, lng))
            return new LocationResolveApiResponse(false, null);

        try
        {
            var resolved = await _resolver.ResolveAsync(request.Lat, request.Lng, cancellationToken);
            return resolved is null
                ? new LocationResolveApiResponse(false, null)
                : new LocationResolveApiResponse(true, resolved);
        }
        catch
        {
            return new LocationResolveApiResponse(false, null);
        }
    }
}

using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Locations;

namespace Matchi.Application.Features.Locations.Resolvers;

public sealed class HybridLocationResolver : ILocationResolver
{
    private readonly IExternalLocationResolver _external;
    private readonly IInternalLocationResolver _internal;
    private readonly LocationResolverOptions _options;

    public HybridLocationResolver(
        IExternalLocationResolver external,
        IInternalLocationResolver @internal,
        LocationResolverOptions options)
    {
        _external = external;
        _internal = @internal;
        _options = options;
    }

    public async Task<LocationResolveResult?> ResolveAsync(
        decimal lat,
        decimal lng,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (ShouldTryExternal())
            {
                var external = await TryResolveAsync(_external, lat, lng, cancellationToken);
                if (external is not null)
                    return external;
            }

            if (ShouldTryInternal())
                return await TryResolveAsync(_internal, lat, lng, cancellationToken);

            return null;
        }
        catch
        {
            return null;
        }
    }

    private bool ShouldTryExternal()
    {
        if (!_options.ExternalEnabled)
            return false;

        return !string.Equals(_options.Mode, "Internal", StringComparison.OrdinalIgnoreCase);
    }

    private bool ShouldTryInternal()
    {
        if (string.Equals(_options.Mode, "External", StringComparison.OrdinalIgnoreCase))
            return _options.FallbackToInternal;

        return _options.FallbackToInternal || !_options.ExternalEnabled
            || string.Equals(_options.Mode, "Internal", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<LocationResolveResult?> TryResolveAsync(
        ILocationResolver resolver,
        decimal lat,
        decimal lng,
        CancellationToken cancellationToken)
    {
        try
        {
            return await resolver.ResolveAsync(lat, lng, cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}

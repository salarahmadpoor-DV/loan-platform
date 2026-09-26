using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Locations;
using Matchi.Application.Features.Locations.Queries;
using Matchi.Application.Features.Locations.Resolvers;
using Matchi.Domain.Locations;
using Matchi.Infrastructure.Locations;

namespace Matchi.Application.Tests;

public sealed class HybridLocationResolverTests
{
    [Fact]
    public async Task External_success_returns_external_result()
    {
        var expected = SampleResult(LocationResolveSources.External);
        var hybrid = new HybridLocationResolver(
            new StubLocationResolver(expected),
            new StubLocationResolver(SampleResult(LocationResolveSources.Internal), throwIfCalled: true),
            AutoOptions());

        var result = await hybrid.ResolveAsync(35.7m, 51.4m);

        Assert.NotNull(result);
        Assert.Equal(LocationResolveSources.External, result.Source);
        Assert.Equal(10, result.CityId);
    }

    [Fact]
    public async Task External_failure_calls_internal()
    {
        var hybrid = new HybridLocationResolver(
            new StubLocationResolver(null, throws: true),
            new StubLocationResolver(SampleResult(LocationResolveSources.Internal)),
            AutoOptions());

        var result = await hybrid.ResolveAsync(35.7m, 51.4m);

        Assert.NotNull(result);
        Assert.Equal(LocationResolveSources.Internal, result.Source);
    }

    [Fact]
    public async Task Both_fail_returns_null()
    {
        var hybrid = new HybridLocationResolver(
            new StubLocationResolver(null),
            new StubLocationResolver(null),
            AutoOptions());

        var result = await hybrid.ResolveAsync(35.7m, 51.4m);

        Assert.Null(result);
    }

    [Fact]
    public async Task Manual_catalog_selection_remains_available_when_resolve_fails()
    {
        var locations = new FakeLocationReadRepository();
        locations.Provinces.Add(new LocationProvince("تهران", "THR").WithId(1));
        locations.Cities.Add(new LocationCity(1, "تهران").WithId(10));
        locations.Districts.Add(new LocationDistrict(10, "منطقه 5").WithId(120));

        var hybrid = new HybridLocationResolver(
            new StubLocationResolver(null),
            new StubLocationResolver(null),
            AutoOptions());

        Assert.Null(await hybrid.ResolveAsync(1m, 1m));

        var provinces = await locations.GetActiveProvincesAsync();
        var cities = await locations.GetActiveCitiesByProvinceIdAsync(1);
        var districts = await locations.GetActiveDistrictsByCityIdAsync(10);

        Assert.Equal("تهران", Assert.Single(provinces).Name);
        Assert.Equal("تهران", Assert.Single(cities).Name);
        Assert.Equal("منطقه 5", Assert.Single(districts).Name);
    }

    private static LocationResolverOptions AutoOptions() => new()
    {
        Mode = "Auto",
        ExternalEnabled = true,
        FallbackToInternal = true,
        TimeoutMs = 1500
    };

    private static LocationResolveResult SampleResult(string source) =>
        new(1, "تهران", 10, "تهران", 120, "منطقه 5", source);

    private sealed class StubLocationResolver : IExternalLocationResolver, IInternalLocationResolver
    {
        private readonly LocationResolveResult? _result;
        private readonly bool _throws;
        private readonly bool _throwIfCalled;

        public StubLocationResolver(
            LocationResolveResult? result,
            bool throws = false,
            bool throwIfCalled = false)
        {
            _result = result;
            _throws = throws;
            _throwIfCalled = throwIfCalled;
        }

        public Task<LocationResolveResult?> ResolveAsync(
            decimal lat,
            decimal lng,
            CancellationToken cancellationToken = default)
        {
            if (_throwIfCalled)
                throw new InvalidOperationException("Internal resolver should not be called.");
            if (_throws)
                throw new InvalidOperationException("upstream failed");

            return Task.FromResult(_result);
        }
    }
}

public sealed class InternalLocationResolverTests
{
    [Fact]
    public async Task Point_inside_city_radius_resolves_internal_location()
    {
        var province = new LocationProvince("تهران", "THR").WithId(1);
        var city = new LocationCity(1, "تهران").WithId(10);
        city.SetCoverage(35.6892m, 51.3890m, 25m);
        city.Set(nameof(LocationCity.Province), province);
        var district = new LocationDistrict(10, "منطقه 5").WithId(120);
        district.SetCoverage(35.75m, 51.32m, 8m);

        var repo = new FakeLocationReadRepository();
        repo.Provinces.Add(province);
        repo.Cities.Add(city);
        repo.Districts.Add(district);

        var resolver = new InternalLocationResolver(repo);
        var result = await resolver.ResolveAsync(35.70m, 51.35m);

        Assert.NotNull(result);
        Assert.Equal(1, result.ProvinceId);
        Assert.Equal(10, result.CityId);
        Assert.Equal(120, result.DistrictId);
        Assert.Equal(LocationResolveSources.Internal, result.Source);
    }

    [Fact]
    public async Task Point_outside_radius_returns_null()
    {
        var province = new LocationProvince("تهران", "THR").WithId(1);
        var city = new LocationCity(1, "تهران").WithId(10);
        city.SetCoverage(35.6892m, 51.3890m, 5m);
        city.Set(nameof(LocationCity.Province), province);

        var repo = new FakeLocationReadRepository();
        repo.Cities.Add(city);

        var resolver = new InternalLocationResolver(repo);
        Assert.Null(await resolver.ResolveAsync(32.0m, 51.0m));
    }
}

public sealed class ResolveLocationQueryTests
{
    [Fact]
    public async Task Resolve_endpoint_returns_success_envelope()
    {
        var handler = new ResolveLocationQueryHandler(
            new FixedResolver(new LocationResolveResult(1, "تهران", 10, "تهران", 120, "منطقه 5", "External")));

        var response = await handler.Handle(new ResolveLocationQuery(35.7m, 51.3m), CancellationToken.None);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(10, response.Data.CityId);
        Assert.Equal("External", response.Data.Source);
    }

    [Fact]
    public async Task Resolve_endpoint_returns_failure_envelope_when_unresolved()
    {
        var handler = new ResolveLocationQueryHandler(new FixedResolver(null));

        var response = await handler.Handle(new ResolveLocationQuery(1m, 1m), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task Invalid_coordinates_return_failure_without_calling_resolver()
    {
        var handler = new ResolveLocationQueryHandler(new FixedResolver(null, throwIfCalled: true));

        var response = await handler.Handle(new ResolveLocationQuery(999m, 0m), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Null(response.Data);
    }

    private sealed class FixedResolver : ILocationResolver
    {
        private readonly LocationResolveResult? _result;
        private readonly bool _throwIfCalled;

        public FixedResolver(LocationResolveResult? result, bool throwIfCalled = false)
        {
            _result = result;
            _throwIfCalled = throwIfCalled;
        }

        public Task<LocationResolveResult?> ResolveAsync(
            decimal lat,
            decimal lng,
            CancellationToken cancellationToken = default)
        {
            if (_throwIfCalled)
                throw new InvalidOperationException("Resolver should not be called.");
            return Task.FromResult(_result);
        }
    }
}

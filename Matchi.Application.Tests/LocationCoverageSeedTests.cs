using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Locations;
using Matchi.Application.Features.Locations.Queries;
using Matchi.Application.Features.Locations.Resolvers;
using Matchi.Domain.Locations;
using Matchi.Infrastructure.Locations;
using Matchi.Infrastructure.Persistence.Seed;

namespace Matchi.Application.Tests;

public sealed class LocationCoverageSeedTests
{
    [Fact]
    public void Blueprint_uses_unique_stable_codes()
    {
        Assert.Equal(8, LocationBlueprint.Provinces.Length);
        Assert.Equal(
            LocationBlueprint.Provinces.Length,
            LocationBlueprint.Provinces.Select(p => p.Code).Distinct(StringComparer.Ordinal).Count());

        Assert.Equal(8, LocationBlueprint.Cities.Length);
        Assert.Equal(
            LocationBlueprint.Cities.Length,
            LocationBlueprint.Cities.Select(c => c.Code).Distinct(StringComparer.Ordinal).Count());

        var districts = LocationBlueprint.Cities.SelectMany(c => c.Districts).ToList();
        Assert.True(districts.Count >= 5);
        Assert.Equal(
            districts.Count,
            districts.Select(d => d.Code).Distinct(StringComparer.Ordinal).Count());

        var provinceCodes = LocationBlueprint.Provinces.Select(p => p.Code).ToHashSet(StringComparer.Ordinal);
        Assert.All(LocationBlueprint.Cities, city => Assert.Contains(city.ProvinceCode, provinceCodes));
    }

    [Fact]
    public async Task Tehran_coordinate_resolves_internally_when_external_is_disabled()
    {
        var locations = SeededLocationRepository.Create();
        var hybrid = new HybridLocationResolver(
            new DisabledExternalResolver(),
            new InternalLocationResolver(locations),
            new LocationResolverOptions
            {
                Mode = "Auto",
                ExternalEnabled = false,
                FallbackToInternal = true,
                TimeoutMs = 1500
            });

        var result = await hybrid.ResolveAsync(35.6892m, 51.3890m);

        Assert.NotNull(result);
        Assert.Equal(LocationResolveSources.Internal, result.Source);
        Assert.Equal("تهران", result.ProvinceName);
        Assert.Equal("تهران", result.CityName);
    }

    [Fact]
    public async Task Unknown_coordinate_returns_null_from_internal_resolver()
    {
        var resolver = new InternalLocationResolver(SeededLocationRepository.Create());
        Assert.Null(await resolver.ResolveAsync(0m, 0m));
    }

    [Fact]
    public async Task Seeded_province_city_district_queries_still_work()
    {
        var locations = SeededLocationRepository.Create();

        var provinces = (await new GetActiveProvincesQueryHandler(locations)
            .Handle(new GetActiveProvincesQuery(), CancellationToken.None)).ToList();
        Assert.Equal(8, provinces.Count);
        var tehran = Assert.Single(provinces, p => p.Code == "THR");

        var cities = (await new GetCitiesByProvinceQueryHandler(locations)
            .Handle(new GetCitiesByProvinceQuery(tehran.Id), CancellationToken.None)).ToList();
        var tehranCity = Assert.Single(cities, c => c.Name == "تهران");

        var districts = (await new GetDistrictsByCityQueryHandler(locations)
            .Handle(new GetDistrictsByCityQuery(tehranCity.Id), CancellationToken.None)).ToList();
        Assert.Contains(districts, d => d.Name == "منطقه 5");
        Assert.Equal(districts.Count, districts.Select(d => d.Name).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public async Task Alborz_seed_includes_karaj_without_invented_neighborhoods()
    {
        var locations = SeededLocationRepository.Create();
        var provinces = (await new GetActiveProvincesQueryHandler(locations)
            .Handle(new GetActiveProvincesQuery(), CancellationToken.None)).ToList();
        var alborz = Assert.Single(provinces, p => p.Code == "ALB");
        Assert.Equal("البرز", alborz.Name);

        var cities = (await new GetCitiesByProvinceQueryHandler(locations)
            .Handle(new GetCitiesByProvinceQuery(alborz.Id), CancellationToken.None)).ToList();
        var karaj = Assert.Single(cities, c => c.Name == "کرج");

        var districts = await new GetDistrictsByCityQueryHandler(locations)
            .Handle(new GetDistrictsByCityQuery(karaj.Id), CancellationToken.None);
        Assert.Empty(districts);
    }

    private sealed class DisabledExternalResolver : IExternalLocationResolver
    {
        public Task<LocationResolveResult?> ResolveAsync(
            decimal lat,
            decimal lng,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("External resolver should be skipped.");
    }
}

internal static class SeededLocationRepository
{
    public static FakeLocationReadRepository Create()
    {
        var repo = new FakeLocationReadRepository();
        var provinces = new Dictionary<string, LocationProvince>(StringComparer.Ordinal);
        long provinceId = 1;
        long cityId = 10;
        long districtId = 100;

        foreach (var seed in LocationBlueprint.Provinces)
        {
            var province = new LocationProvince(seed.Name, seed.Code).WithId(provinceId++);
            provinces[seed.Code] = province;
            repo.Provinces.Add(province);
        }

        foreach (var seed in LocationBlueprint.Cities)
        {
            var province = provinces[seed.ProvinceCode];
            var city = new LocationCity(province.Id, seed.Name, seed.Code).WithId(cityId++);
            city.SetCoverage(seed.Coverage.CenterLat, seed.Coverage.CenterLng, seed.Coverage.RadiusKm);
            city.Set(nameof(LocationCity.Province), province);
            repo.Cities.Add(city);

            foreach (var districtSeed in seed.Districts)
            {
                var district = new LocationDistrict(city.Id, districtSeed.Name, districtSeed.Code).WithId(districtId++);
                district.SetCoverage(
                    districtSeed.Coverage.CenterLat,
                    districtSeed.Coverage.CenterLng,
                    districtSeed.Coverage.RadiusKm);
                repo.Districts.Add(district);
            }
        }

        return repo;
    }
}

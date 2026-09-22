using Matchi.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Matchi.Infrastructure.Persistence.Seed;

public static class LocationSeeder
{
    public static async Task SeedAsync(MatchiDbContext context, ILogger logger)
    {
        var provinces = new Dictionary<string, LocationProvince>(StringComparer.Ordinal);
        foreach (var seed in LocationBlueprint.Provinces)
        {
            var province = await EnsureProvinceAsync(context, seed, logger);
            provinces[seed.Code] = province;
        }

        foreach (var seed in LocationBlueprint.Cities)
        {
            if (!provinces.TryGetValue(seed.ProvinceCode, out var province))
            {
                logger.LogWarning(
                    "Location city {Code} skipped because province {Province} was not found.",
                    seed.Code,
                    seed.ProvinceCode);
                continue;
            }

            var city = await EnsureCityAsync(context, province.Id, seed, logger);
            foreach (var district in seed.Districts)
                await EnsureDistrictAsync(context, city.Id, district, logger);
        }
    }

    private static async Task<LocationProvince> EnsureProvinceAsync(
        MatchiDbContext context,
        LocationProvinceSeed seed,
        ILogger logger)
    {
        var province = await context.LocationProvinces
            .FirstOrDefaultAsync(x => x.Code == seed.Code);

        if (province is null)
        {
            province = new LocationProvince(seed.Name, seed.Code);
            await context.LocationProvinces.AddAsync(province);
            await context.SaveChangesAsync();
            return province;
        }

        if (!string.Equals(province.Name, seed.Name, StringComparison.Ordinal))
        {
            logger.LogWarning(
                "Location province code {Code} already exists as '{Existing}'; seed name '{Seed}' was not applied.",
                seed.Code,
                province.Name,
                seed.Name);
        }

        return province;
    }

    private static async Task<LocationCity> EnsureCityAsync(
        MatchiDbContext context,
        long provinceId,
        LocationCitySeed seed,
        ILogger logger)
    {
        var city = await context.LocationCities
            .FirstOrDefaultAsync(x => x.Code == seed.Code);

        if (city is null)
        {
            city = new LocationCity(provinceId, seed.Name, seed.Code);
            city.SetCoverage(seed.Coverage.CenterLat, seed.Coverage.CenterLng, seed.Coverage.RadiusKm);
            await context.LocationCities.AddAsync(city);
            await context.SaveChangesAsync();
            return city;
        }

        if (city.ProvinceId != provinceId)
        {
            logger.LogWarning(
                "Location city code {Code} already exists under another province; seed was not moved.",
                seed.Code);
        }
        else if (!string.Equals(city.Name, seed.Name, StringComparison.Ordinal)
                 || city.CenterLat != seed.Coverage.CenterLat
                 || city.CenterLng != seed.Coverage.CenterLng
                 || city.RadiusKm != seed.Coverage.RadiusKm)
        {
            logger.LogWarning(
                "Location city code {Code} already exists; seed name/coverage was not overwritten.",
                seed.Code);
        }

        return city;
    }

    private static async Task EnsureDistrictAsync(
        MatchiDbContext context,
        long cityId,
        LocationDistrictSeed seed,
        ILogger logger)
    {
        var district = await context.LocationDistricts
            .FirstOrDefaultAsync(x => x.Code == seed.Code);

        if (district is null)
        {
            district = new LocationDistrict(cityId, seed.Name, seed.Code);
            district.SetCoverage(seed.Coverage.CenterLat, seed.Coverage.CenterLng, seed.Coverage.RadiusKm);
            await context.LocationDistricts.AddAsync(district);
            await context.SaveChangesAsync();
            return;
        }

        if (district.CityId != cityId)
        {
            logger.LogWarning(
                "Location district code {Code} already exists under another city; seed was not moved.",
                seed.Code);
        }
        else if (!string.Equals(district.Name, seed.Name, StringComparison.Ordinal)
                 || district.CenterLat != seed.Coverage.CenterLat
                 || district.CenterLng != seed.Coverage.CenterLng
                 || district.RadiusKm != seed.Coverage.RadiusKm)
        {
            logger.LogWarning(
                "Location district code {Code} already exists; seed name/coverage was not overwritten.",
                seed.Code);
        }
    }
}

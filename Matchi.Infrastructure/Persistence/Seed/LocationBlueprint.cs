namespace Matchi.Infrastructure.Persistence.Seed;

internal sealed record LocationCoverageSeed(
    decimal CenterLat,
    decimal CenterLng,
    decimal RadiusKm);

internal sealed record LocationDistrictSeed(
    string Code,
    string Name,
    LocationCoverageSeed Coverage);

internal sealed record LocationCitySeed(
    string Code,
    string Name,
    string ProvinceCode,
    LocationCoverageSeed Coverage,
    LocationDistrictSeed[] Districts);

internal sealed record LocationProvinceSeed(
    string Code,
    string Name);

internal static class LocationBlueprint
{
    public static readonly LocationProvinceSeed[] Provinces =
    [
        new("THR", "تهران"),
        new("ALB", "البرز"),
        new("ESF", "اصفهان"),
        new("FRS", "فارس"),
        new("RKH", "خراسان رضوی"),
        new("EAZ", "آذربایجان شرقی"),
        new("GIL", "گیلان"),
        new("MZN", "مازندران")
    ];

    public static readonly LocationCitySeed[] Cities =
    [
        City("tehran", "تهران", "THR", 35.6892m, 51.3890m, 32m,
            D("tehran-d1", "منطقه 1", 35.8070m, 51.4280m, 5.5m),
            D("tehran-d2", "منطقه 2", 35.7770m, 51.3600m, 5.5m),
            D("tehran-d3", "منطقه 3", 35.7570m, 51.4350m, 4.5m),
            D("tehran-d5", "منطقه 5", 35.7600m, 51.3100m, 6.0m),
            D("tehran-d22", "منطقه 22", 35.7400m, 51.2100m, 8.0m)),
        City("karaj", "کرج", "ALB", 35.8400m, 50.9391m, 18m),
        City("isfahan", "اصفهان", "ESF", 32.6546m, 51.6680m, 22m),
        City("shiraz", "شیراز", "FRS", 29.5918m, 52.5836m, 20m),
        City("mashhad", "مشهد", "RKH", 36.2605m, 59.6168m, 22m),
        City("tabriz", "تبریز", "EAZ", 38.0962m, 46.2738m, 18m),
        City("rasht", "رشت", "GIL", 37.2808m, 49.5832m, 12m),
        City("sari", "ساری", "MZN", 36.5633m, 53.0601m, 12m)
    ];

    private static LocationCitySeed City(
        string code,
        string name,
        string provinceCode,
        decimal lat,
        decimal lng,
        decimal radiusKm,
        params LocationDistrictSeed[] districts) =>
        new(code, name, provinceCode, new LocationCoverageSeed(lat, lng, radiusKm), districts);

    private static LocationDistrictSeed D(
        string code,
        string name,
        decimal lat,
        decimal lng,
        decimal radiusKm) =>
        new(code, name, new LocationCoverageSeed(lat, lng, radiusKm));
}

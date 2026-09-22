using Matchi.Domain.Locations;

namespace Matchi.Domain.Interfaces;

public interface ILocationReadRepository
{
    Task<IReadOnlyList<LocationProvince>> GetActiveProvincesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LocationCity>> GetActiveCitiesByProvinceIdAsync(
        long provinceId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LocationCity>> GetActiveCitiesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LocationDistrict>> GetActiveDistrictsByCityIdAsync(
        long cityId,
        CancellationToken cancellationToken = default);

    Task<LocationProvince?> FindProvinceByIdAsync(
        long provinceId,
        CancellationToken cancellationToken = default);

    Task<LocationCity?> FindCityByIdAsync(
        long cityId,
        CancellationToken cancellationToken = default);

    Task<LocationDistrict?> FindDistrictByIdAsync(
        long districtId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LocationCity>> GetActiveCoveredCitiesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LocationDistrict>> GetActiveCoveredDistrictsByCityIdAsync(
        long cityId,
        CancellationToken cancellationToken = default);
}

using Matchi.Application.Features.Locations.Queries;
using Matchi.Domain.Locations;

namespace Matchi.Application.Tests;

public sealed class LocationQueryTests
{
    [Fact]
    public async Task GetActiveProvinces_maps_name_and_code()
    {
        var repo = new FakeLocationReadRepository();
        repo.Provinces.Add(new LocationProvince("Tehran", "THR").WithId(1));

        var handler = new GetActiveProvincesQueryHandler(repo);
        var result = await handler.Handle(new GetActiveProvincesQuery(), CancellationToken.None);

        var province = Assert.Single(result);
        Assert.Equal(1, province.Id);
        Assert.Equal("Tehran", province.Name);
        Assert.Equal("THR", province.Code);
    }

    [Fact]
    public async Task GetCitiesByProvince_maps_province_id()
    {
        var repo = new FakeLocationReadRepository();
        repo.Cities.Add(new LocationCity(2, "Karaj").WithId(10));

        var handler = new GetCitiesByProvinceQueryHandler(repo);
        var result = await handler.Handle(new GetCitiesByProvinceQuery(2), CancellationToken.None);

        var city = Assert.Single(result);
        Assert.Equal(10, city.Id);
        Assert.Equal(2, city.ProvinceId);
        Assert.Equal("Karaj", city.Name);
    }

    [Fact]
    public async Task GetDistrictsByCity_maps_city_id()
    {
        var repo = new FakeLocationReadRepository();
        repo.Districts.Add(new LocationDistrict(10, "District 1").WithId(20));

        var handler = new GetDistrictsByCityQueryHandler(repo);
        var result = await handler.Handle(new GetDistrictsByCityQuery(10), CancellationToken.None);

        var district = Assert.Single(result);
        Assert.Equal(20, district.Id);
        Assert.Equal(10, district.CityId);
        Assert.Equal("District 1", district.Name);
    }
}

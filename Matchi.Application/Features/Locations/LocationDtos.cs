namespace Matchi.Application.Features.Locations;

public sealed class ProvinceDto
{
    public long Id { get; init; }

    public string Name { get; init; } = null!;

    public string Code { get; init; } = null!;
}

public sealed class CityDto
{
    public long Id { get; init; }

    public long ProvinceId { get; init; }

    public string Name { get; init; } = null!;
}

public sealed class DistrictDto
{
    public long Id { get; init; }

    public long CityId { get; init; }

    public string Name { get; init; } = null!;
}

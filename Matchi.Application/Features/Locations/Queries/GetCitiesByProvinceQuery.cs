using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Locations.Queries;

public sealed record GetCitiesByProvinceQuery(long ProvinceId) : IRequest<IReadOnlyList<CityDto>>;

public sealed class GetCitiesByProvinceQueryHandler
    : IRequestHandler<GetCitiesByProvinceQuery, IReadOnlyList<CityDto>>
{
    private readonly ILocationReadRepository _locations;

    public GetCitiesByProvinceQueryHandler(ILocationReadRepository locations)
    {
        _locations = locations;
    }

    public async Task<IReadOnlyList<CityDto>> Handle(
        GetCitiesByProvinceQuery request,
        CancellationToken cancellationToken)
    {
        var cities = await _locations.GetActiveCitiesByProvinceIdAsync(
            request.ProvinceId,
            cancellationToken);

        return cities
            .Select(x => new CityDto
            {
                Id = x.Id,
                ProvinceId = x.ProvinceId,
                Name = x.Name
            })
            .ToList();
    }
}

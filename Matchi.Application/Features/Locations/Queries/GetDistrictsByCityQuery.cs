using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Locations.Queries;

public sealed record GetDistrictsByCityQuery(long CityId) : IRequest<IReadOnlyList<DistrictDto>>;

public sealed class GetDistrictsByCityQueryHandler
    : IRequestHandler<GetDistrictsByCityQuery, IReadOnlyList<DistrictDto>>
{
    private readonly ILocationReadRepository _locations;

    public GetDistrictsByCityQueryHandler(ILocationReadRepository locations)
    {
        _locations = locations;
    }

    public async Task<IReadOnlyList<DistrictDto>> Handle(
        GetDistrictsByCityQuery request,
        CancellationToken cancellationToken)
    {
        var districts = await _locations.GetActiveDistrictsByCityIdAsync(
            request.CityId,
            cancellationToken);

        return districts
            .Select(x => new DistrictDto
            {
                Id = x.Id,
                CityId = x.CityId,
                Name = x.Name
            })
            .ToList();
    }
}

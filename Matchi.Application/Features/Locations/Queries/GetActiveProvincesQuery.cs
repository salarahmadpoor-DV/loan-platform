using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Locations.Queries;

public sealed record GetActiveProvincesQuery : IRequest<IReadOnlyList<ProvinceDto>>;

public sealed class GetActiveProvincesQueryHandler
    : IRequestHandler<GetActiveProvincesQuery, IReadOnlyList<ProvinceDto>>
{
    private readonly ILocationReadRepository _locations;

    public GetActiveProvincesQueryHandler(ILocationReadRepository locations)
    {
        _locations = locations;
    }

    public async Task<IReadOnlyList<ProvinceDto>> Handle(
        GetActiveProvincesQuery request,
        CancellationToken cancellationToken)
    {
        var provinces = await _locations.GetActiveProvincesAsync(cancellationToken);

        return provinces
            .Select(x => new ProvinceDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code
            })
            .ToList();
    }
}

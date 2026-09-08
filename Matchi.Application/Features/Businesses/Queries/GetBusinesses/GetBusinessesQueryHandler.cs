using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Queries.GetBusinesses;

public sealed class GetBusinessesQueryHandler : IRequestHandler<GetBusinessesQuery, IEnumerable<BusinessDto>>
{
    private readonly IBusinessRepository _businesses;

    public GetBusinessesQueryHandler(IBusinessRepository businesses)
    {
        _businesses = businesses;
    }

    public async Task<IEnumerable<BusinessDto>> Handle(
        GetBusinessesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _businesses.ListAsync(cancellationToken);
        return items.Select(b => new BusinessDto(b.Id, b.Name, b.Address));
    }
}

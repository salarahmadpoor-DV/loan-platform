using MediatR;

namespace Matchi.Application.Features.Businesses.Queries.GetBusinesses;

public sealed class GetBusinessesQueryHandler : IRequestHandler<GetBusinessesQuery, IEnumerable<BusinessDto>>
{
    public Task<IEnumerable<BusinessDto>> Handle(
        GetBusinessesQuery request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult((IEnumerable<BusinessDto>)Array.Empty<BusinessDto>());
    }
}

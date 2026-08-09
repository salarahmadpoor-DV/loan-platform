using MediatR;

namespace Matchi.Application.Features.Businesses.Queries.GetBusinesses;

public sealed record GetBusinessesQuery(int Page, int PageSize) : IRequest<IEnumerable<BusinessDto>>;

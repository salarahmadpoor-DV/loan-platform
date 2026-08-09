using MediatR;

namespace Matchi.Application.Features.Requests.Queries.GetServiceRequestsByUser;

public sealed record GetServiceRequestsByUserQuery(long UserId) : IRequest<IEnumerable<ServiceRequestSummaryDto>>;

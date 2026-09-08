using MediatR;

namespace Matchi.Application.Features.Requests.Queries.GetMyRequests;

public sealed record GetMyRequestsQuery : IRequest<IReadOnlyList<RequestDto>>;

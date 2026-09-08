using MediatR;

namespace Matchi.Application.Features.Requests.Queries.GetRequestById;

public sealed record GetRequestByIdQuery(long RequestId) : IRequest<RequestDto?>;

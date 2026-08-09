using MediatR;

namespace Matchi.Application.Features.Requests.Queries.GetServiceRequestById;

public sealed record GetServiceRequestByIdQuery(long RequestId) : IRequest<ServiceRequestDto?>;

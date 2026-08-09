using MediatR;

namespace Matchi.Application.Features.Services.Queries.GetServiceById;

public sealed record GetServiceByIdQuery(long ServiceId) : IRequest<ServiceDetailDto?>;

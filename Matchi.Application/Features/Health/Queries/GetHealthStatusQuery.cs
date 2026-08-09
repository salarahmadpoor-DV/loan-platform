using MediatR;

namespace Matchi.Application.Features.Health.Queries;

public sealed record GetHealthStatusQuery : IRequest<HealthStatusDto>;

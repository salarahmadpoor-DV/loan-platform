using MediatR;

namespace Matchi.Application.Features.Health.Queries;

public sealed class GetHealthStatusQueryHandler : IRequestHandler<GetHealthStatusQuery, HealthStatusDto>
{
    public Task<HealthStatusDto> Handle(
        GetHealthStatusQuery request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new HealthStatusDto("Healthy"));
    }
}

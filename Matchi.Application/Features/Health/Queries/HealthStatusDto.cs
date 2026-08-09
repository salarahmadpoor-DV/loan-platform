namespace Matchi.Application.Features.Health.Queries;

public sealed class HealthStatusDto
{
    public string Status { get; init; } = null!;

    public HealthStatusDto(string status)
    {
        Status = status;
    }
}

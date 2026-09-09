namespace Matchi.Application.Features.Executions;

public sealed record ServiceExecutionDto(
    long Id,
    long DealId,
    long? BusinessId,
    string Status,
    DateOnly? ScheduledDate,
    TimeSpan? ScheduledTimeFrom,
    TimeSpan? ScheduledTimeTo,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime CreateDate);

public sealed record ExecutionAssignmentDto(
    long Id,
    long ServiceExecutionId,
    long ProviderId,
    string Role,
    bool IsPrimary,
    string Status,
    DateTime AssignedAt);

internal static class ExecutionDtoMapper
{
    public static ServiceExecutionDto ToDto(Domain.Entities.ServiceExecution e) =>
        new(
            e.Id,
            e.DealId,
            e.BusinessId,
            e.Status,
            e.ScheduledDate,
            e.ScheduledTimeFrom,
            e.ScheduledTimeTo,
            e.StartedAt,
            e.CompletedAt,
            e.CreateDate);

    public static ExecutionAssignmentDto ToDto(Domain.Entities.ExecutionAssignment a) =>
        new(
            a.Id,
            a.ServiceExecutionId,
            a.ProviderId,
            a.Role,
            a.IsPrimary,
            a.Status,
            a.AssignedAt);
}

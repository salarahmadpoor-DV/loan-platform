using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ServiceExecution : TimestampedEntity
{
    public long DealId { get; private set; }

    public long? BusinessId { get; private set; }

    public string Status { get; private set; } = "Pending";

    public DateOnly? ScheduledDate { get; private set; }

    public TimeSpan? ScheduledTimeFrom { get; private set; }

    public TimeSpan? ScheduledTimeTo { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public Business? Business { get; private set; }

    public Deal Deal { get; private set; } = null!;

    public ICollection<ExecutionAssignment> Assignments { get; private set; } = new List<ExecutionAssignment>();

    private ServiceExecution()
    {
    }

    private ServiceExecution(
        long dealId,
        long? businessId,
        DateOnly? scheduledDate,
        TimeSpan? scheduledTimeFrom,
        TimeSpan? scheduledTimeTo)
        : this(dealId, businessId, scheduledDate, scheduledTimeFrom, scheduledTimeTo, deal: null)
    {
    }

    private ServiceExecution(
        Deal deal,
        long? businessId,
        DateOnly? scheduledDate,
        TimeSpan? scheduledTimeFrom,
        TimeSpan? scheduledTimeTo)
        : this(deal.Id, businessId, scheduledDate, scheduledTimeFrom, scheduledTimeTo, deal)
    {
    }

    private ServiceExecution(
        long dealId,
        long? businessId,
        DateOnly? scheduledDate,
        TimeSpan? scheduledTimeFrom,
        TimeSpan? scheduledTimeTo,
        Deal? deal)
    {
        if (deal is null && dealId <= 0)
            throw new InvalidOperationException("DealId must be greater than zero.");
        if (businessId is <= 0)
            throw new InvalidOperationException("BusinessId must be greater than zero when supplied.");
        EnsureSchedule(scheduledTimeFrom, scheduledTimeTo);

        Deal = deal!;
        DealId = dealId;
        BusinessId = businessId;
        ScheduledDate = scheduledDate;
        ScheduledTimeFrom = scheduledTimeFrom;
        ScheduledTimeTo = scheduledTimeTo;
        Status = "Pending";
    }

    public static ServiceExecution Create(
        long dealId,
        long? businessId,
        DateOnly? scheduledDate = null,
        TimeSpan? scheduledTimeFrom = null,
        TimeSpan? scheduledTimeTo = null)
    {
        return new ServiceExecution(dealId, businessId, scheduledDate, scheduledTimeFrom, scheduledTimeTo);
    }

    public static ServiceExecution CreateForDeal(
        Deal deal,
        long? businessId,
        DateOnly? scheduledDate = null,
        TimeSpan? scheduledTimeFrom = null,
        TimeSpan? scheduledTimeTo = null)
    {
        ArgumentNullException.ThrowIfNull(deal);
        var execution = new ServiceExecution(deal, businessId, scheduledDate, scheduledTimeFrom, scheduledTimeTo);
        deal.ServiceExecutions.Add(execution);
        return execution;
    }

    public void UpdateSchedule(DateOnly? scheduledDate, TimeSpan? scheduledTimeFrom, TimeSpan? scheduledTimeTo)
    {
        if (!string.Equals(Status, "Pending", StringComparison.Ordinal))
            throw new InvalidOperationException("Only a pending execution can be scheduled.");

        EnsureSchedule(scheduledTimeFrom, scheduledTimeTo);
        ScheduledDate = scheduledDate;
        ScheduledTimeFrom = scheduledTimeFrom;
        ScheduledTimeTo = scheduledTimeTo;
        SetUpdated();
    }

    public void Start()
    {
        if (!string.Equals(Status, "Pending", StringComparison.Ordinal))
            throw new InvalidOperationException("Only a pending execution can be started.");

        Status = "InProgress";
        StartedAt = DateTime.UtcNow;
        SetUpdated();
    }

    public void Complete()
    {
        if (!string.Equals(Status, "InProgress", StringComparison.Ordinal))
            throw new InvalidOperationException("Only an in-progress execution can be completed.");
        if (!StartedAt.HasValue)
            throw new InvalidOperationException("An execution cannot be completed before it is started.");

        Status = "Completed";
        CompletedAt = DateTime.UtcNow;
        SetUpdated();
    }

    public void Cancel()
    {
        if (string.Equals(Status, "Completed", StringComparison.Ordinal)
            || string.Equals(Status, "Cancelled", StringComparison.Ordinal))
            throw new InvalidOperationException("A completed or cancelled execution cannot be cancelled.");

        Status = "Cancelled";
        SetUpdated();
    }

    private static void EnsureSchedule(TimeSpan? from, TimeSpan? to)
    {
        if (from is not null && to is not null && from >= to)
            throw new InvalidOperationException("ScheduledTimeFrom must be earlier than ScheduledTimeTo.");
    }
}

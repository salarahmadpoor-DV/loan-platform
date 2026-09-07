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

    public ServiceExecution(long dealId)
    {
        DealId = dealId;
    }
}

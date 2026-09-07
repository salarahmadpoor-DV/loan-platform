using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class ExecutionAssignment : TimestampedEntity
{
    public long ServiceExecutionId { get; private set; }

    public long ProviderId { get; private set; }

    public string Role { get; private set; } = null!;

    public bool IsPrimary { get; private set; } = false;

    public string Status { get; private set; } = "Assigned";

    public DateTime AssignedAt { get; private set; }

    public DateTime? StartAt { get; private set; }

    public DateTime? EndAt { get; private set; }

    public Provider Provider { get; private set; } = null!;

    public ServiceExecution ServiceExecution { get; private set; } = null!;

    private ExecutionAssignment()
    {
    }

    public ExecutionAssignment(long serviceExecutionId, long providerId, string role)
    {
        ServiceExecutionId = serviceExecutionId;
        ProviderId = providerId;
        Role = role;
    }
}

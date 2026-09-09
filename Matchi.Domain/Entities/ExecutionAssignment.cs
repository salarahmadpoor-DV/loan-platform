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

    private ExecutionAssignment(long serviceExecutionId, long providerId, string role, bool isPrimary)
    {
        if (serviceExecutionId <= 0)
            throw new InvalidOperationException("ServiceExecutionId must be greater than zero.");
        if (providerId <= 0)
            throw new InvalidOperationException("ProviderId must be greater than zero.");
        if (string.IsNullOrWhiteSpace(role))
            throw new InvalidOperationException("Role is required.");
        if (role.Length > 100)
            throw new InvalidOperationException("Role must be at most 100 characters.");

        ServiceExecutionId = serviceExecutionId;
        ProviderId = providerId;
        Role = role.Trim();
        IsPrimary = isPrimary;
        Status = "Assigned";
        AssignedAt = DateTime.UtcNow;
    }

    public static ExecutionAssignment Create(
        long serviceExecutionId,
        long providerId,
        string role,
        bool isPrimary)
    {
        return new ExecutionAssignment(serviceExecutionId, providerId, role, isPrimary);
    }

    public void Cancel()
    {
        if (string.Equals(Status, "Cancelled", StringComparison.Ordinal))
            throw new InvalidOperationException("Assignment is already cancelled.");

        Status = "Cancelled";
        SetUpdated();
    }
}

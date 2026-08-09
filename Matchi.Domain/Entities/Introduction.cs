using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Introduction : AuditableEntity
{
    public long ServiceRequestId { get; private set; }
    public ServiceRequest ServiceRequest { get; private set; } = null!;

    public string TargetType { get; private set; } = null!; // "Provider" | "Business"
    public long TargetId { get; private set; }

    public string Status { get; private set; } = "Pending";

    public long? AssignedProviderId { get; private set; }

    private Introduction() { }

    public Introduction(long requestId, string targetType, long targetId, string? source = null)
    {
        ServiceRequestId = requestId;
        TargetType = targetType;
        TargetId = targetId;
        Status = "Pending";
    }

    public void AssignProvider(long providerId)
    {
        AssignedProviderId = providerId;
        Status = "Assigned";
    }

    public void Confirm(string status)
    {
        Status = status;
    }
}
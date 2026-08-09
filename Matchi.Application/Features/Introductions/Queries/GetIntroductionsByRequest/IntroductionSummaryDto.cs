namespace Matchi.Application.Features.Introductions.Queries.GetIntroductionsByRequest;

public sealed class IntroductionSummaryDto
{
    public long Id { get; init; }
    public long RequestId { get; init; }
    public string TargetType { get; init; } = null!;
    public long TargetId { get; init; }
    public string Status { get; init; } = null!;
    public long? AssignedProviderId { get; init; }

    public IntroductionSummaryDto(long id, long requestId, string targetType, long targetId, string status, long? assignedProviderId)
    {
        Id = id;
        RequestId = requestId;
        TargetType = targetType;
        TargetId = targetId;
        Status = status;
        AssignedProviderId = assignedProviderId;
    }
}

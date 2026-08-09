using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class Review : AuditableEntity
{
    public long IntroductionId { get; private set; }
    public Introduction Introduction { get; private set; } = null!;

    public string TargetType { get; private set; } = null!; // Provider | Business
    public long TargetId { get; private set; }

    public int Rating { get; private set; }
    public string? Comment { get; private set; }

    private Review() { }

    public Review(long introductionId, string targetType, long targetId, int rating, string? comment)
    {
        IntroductionId = introductionId;
        TargetType = targetType;
        TargetId = targetId;
        Rating = rating;
        Comment = comment;
    }
}
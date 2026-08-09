namespace Matchi.Application.Features.Reviews.Queries;

public sealed class ReviewDto
{
    public long Id { get; init; }
    public long TargetId { get; init; }
    public string TargetType { get; init; } = null!;
    public int Rating { get; init; }
    public string? Comment { get; init; }

    public ReviewDto(long id, long targetId, string targetType, int rating, string? comment)
    {
        Id = id;
        TargetId = targetId;
        TargetType = targetType;
        Rating = rating;
        Comment = comment;
    }
}

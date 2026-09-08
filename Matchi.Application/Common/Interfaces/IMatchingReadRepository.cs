namespace Matchi.Application.Common.Interfaces;

public enum MatchCandidateType
{
    Provider = 0,
    Business = 1
}

public sealed class MatchingCriteria
{
    public IReadOnlyList<long> ServiceIds { get; init; } = Array.Empty<long>();
    public IReadOnlyList<long> ProductIds { get; init; } = Array.Empty<long>();
    public IReadOnlyList<long> ProductCategoryIds { get; init; } = Array.Empty<long>();
    public IReadOnlyList<long> ServiceAttributeIds { get; init; } = Array.Empty<long>();
    public string? Province { get; init; }
    public string? City { get; init; }
    public string? District { get; init; }
    public byte? DayOfWeek { get; init; }
    public TimeSpan? TimeFrom { get; init; }
    public TimeSpan? TimeTo { get; init; }
    public bool IsFlexible { get; init; }
    public bool RequireService { get; init; }
    public bool RequireProduct { get; init; }
}

public sealed record MatchingCandidateRow(
    MatchCandidateType CandidateType,
    long CandidateId,
    string DisplayName,
    int Score);

public interface IMatchingReadRepository
{
    Task<IReadOnlyList<MatchingCandidateRow>> FindProviderMatchesAsync(
        MatchingCriteria criteria,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MatchingCandidateRow>> FindBusinessMatchesAsync(
        MatchingCriteria criteria,
        CancellationToken cancellationToken = default);
}

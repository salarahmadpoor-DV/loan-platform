using Matchi.Application.Common.Interfaces;

namespace Matchi.Application.Features.Matching;

public static class MatchingScores
{
    public const int Service = 50;
    public const int Product = 20;
    public const int Capability = 15;
    public const int Area = 10;
    public const int Availability = 5;
    public const int MaxResults = 50;
}

public static class MatchingRanker
{
    public static IReadOnlyList<MatchResultDto> Rank(IEnumerable<MatchingCandidateRow> candidates)
    {
        return candidates
            .GroupBy(c => (c.CandidateType, c.CandidateId))
            .Select(g => g
                .OrderBy(c => c.DistanceKm ?? double.MaxValue)
                .ThenByDescending(c => c.Score)
                .First())
            .OrderBy(c => c.DistanceKm.HasValue ? 0 : 1)
            .ThenBy(c => c.DistanceKm)
            .ThenByDescending(c => c.Score)
            .ThenBy(c => c.CandidateType)
            .ThenBy(c => c.CandidateId)
            .Take(MatchingScores.MaxResults)
            .Select((c, index) => new MatchResultDto(
                c.CandidateType == MatchCandidateType.Provider ? "Provider" : "Business",
                c.CandidateId,
                c.DisplayName,
                c.Score,
                index + 1,
                c.DistanceKm))
            .ToList();
    }

    public static int ComputeScore(
        bool serviceMatch,
        bool productMatch,
        bool capabilityMatch,
        bool areaMatch,
        bool availabilityMatch)
    {
        var score = 0;
        if (serviceMatch)
            score += MatchingScores.Service;
        if (productMatch)
            score += MatchingScores.Product;
        if (capabilityMatch)
            score += MatchingScores.Capability;
        if (areaMatch)
            score += MatchingScores.Area;
        if (availabilityMatch)
            score += MatchingScores.Availability;
        return score;
    }
}

public sealed record MatchResultDto(
    string CandidateType,
    long CandidateId,
    string DisplayName,
    int Score,
    int Rank,
    double? DistanceKm = null);

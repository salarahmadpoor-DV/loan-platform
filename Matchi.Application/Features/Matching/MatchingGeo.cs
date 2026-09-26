using Matchi.Application.Common.Interfaces;

namespace Matchi.Application.Features.Matching;

public sealed record ServiceAreaGeo(
    long OwnerId,
    decimal? Lat,
    decimal? Lng,
    decimal? Radius,
    bool IsActive,
    bool IsDeleted);

/// <summary>
/// Applies radius containment to existing matching candidates.
/// Does not join BusinessProvider. OwnerId is Provider.Id or Business.Id from the candidate row.
/// </summary>
public static class MatchingGeo
{
    public static IReadOnlyList<MatchingCandidateRow> Apply(
        IReadOnlyList<MatchingCandidateRow> candidates,
        IReadOnlyList<ServiceAreaGeo> areas,
        double? requestLat,
        double? requestLng)
    {
        if (!GeoDistance.IsValidPoint(requestLat, requestLng))
            return candidates;

        var lat = requestLat!.Value;
        var lng = requestLng!.Value;

        return candidates
            .Select(candidate =>
            {
                var distanceKm = NearestContainingDistanceKm(lat, lng, candidate.CandidateId, areas);
                var score = candidate.Score;
                var areaMatch = candidate.AreaMatch;
                if (distanceKm.HasValue && !areaMatch)
                {
                    score += MatchingScores.Area;
                    areaMatch = true;
                }

                return candidate with
                {
                    Score = score,
                    AreaMatch = areaMatch,
                    DistanceKm = distanceKm
                };
            })
            .ToList();
    }

    public static double? NearestContainingDistanceKm(
        double requestLat,
        double requestLng,
        long ownerId,
        IEnumerable<ServiceAreaGeo> areas)
    {
        double? nearest = null;
        foreach (var area in areas)
        {
            if (area.OwnerId != ownerId || !IsUsable(area))
                continue;

            var distanceKm = GeoDistance.HaversineKm(
                requestLat,
                requestLng,
                (double)area.Lat!.Value,
                (double)area.Lng!.Value);

            if (distanceKm > (double)area.Radius!.Value)
                continue;

            if (nearest is null || distanceKm < nearest.Value)
                nearest = distanceKm;
        }

        return nearest;
    }

    private static bool IsUsable(ServiceAreaGeo area)
    {
        if (area.IsDeleted || !area.IsActive || area.Radius is null || area.Radius <= 0)
            return false;

        return GeoDistance.IsValidPoint(
            area.Lat is null ? null : (double)area.Lat.Value,
            area.Lng is null ? null : (double)area.Lng.Value);
    }
}

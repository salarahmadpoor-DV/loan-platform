using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Matching;
using Matchi.Domain.Entities;
using Matchi.Domain.Locations;

namespace Matchi.Application.Tests;

public sealed class GeoDistanceTests
{
    [Fact]
    public void HaversineKm_SamePoint_IsZero()
    {
        Assert.Equal(0, GeoDistance.HaversineKm(35.6892, 51.389, 35.6892, 51.389), 6);
    }

    [Fact]
    public void HaversineKm_OneDegreeLongitudeOnEquator_IsAbout111Km()
    {
        var km = GeoDistance.HaversineKm(0, 0, 0, 1);
        Assert.InRange(km, 110.5, 111.4);
    }

    [Fact]
    public void IsValidPoint_RejectsIncompleteAndOutOfRange()
    {
        Assert.False(GeoDistance.IsValidPoint(null, 51));
        Assert.False(GeoDistance.IsValidPoint(35, null));
        Assert.False(GeoDistance.IsValidPoint(91, 0));
        Assert.False(GeoDistance.IsValidPoint(0, 181));
        Assert.True(GeoDistance.IsValidPoint(35.7, 51.4));
    }
}

public sealed class MatchingGeoTests
{
    private const double RequestLat = 35.6892;
    private const double RequestLng = 51.389;

    [Fact]
    public void InsideRadius_SetsDistance_AndAddsAreaScore()
    {
        var candidate = Row(1, score: MatchingScores.Service);
        var inside = Area(1, RequestLat, RequestLng, radiusKm: 5);
        var result = MatchingGeo.Apply([candidate], [inside], RequestLat, RequestLng);

        Assert.Single(result);
        Assert.True(result[0].AreaMatch);
        Assert.Equal(MatchingScores.Service + MatchingScores.Area, result[0].Score);
        Assert.NotNull(result[0].DistanceKm);
        Assert.True(result[0].DistanceKm < 0.01);
    }

    [Fact]
    public void OutsideRadius_DoesNotSetDistance()
    {
        var far = Area(1, RequestLat + 1, RequestLng, radiusKm: 5);
        var result = MatchingGeo.Apply(
            [Row(1, MatchingScores.Service)],
            [far],
            RequestLat,
            RequestLng);

        Assert.Null(result[0].DistanceKm);
        Assert.False(result[0].AreaMatch);
        Assert.Equal(MatchingScores.Service, result[0].Score);
    }

    [Fact]
    public void MultipleAreas_UsesNearestContaining_AndDoesNotDuplicateProvider()
    {
        var areas = new[]
        {
            Area(1, RequestLat + 0.2, RequestLng, radiusKm: 5),
            Area(1, RequestLat, RequestLng, radiusKm: 20),
            Area(1, RequestLat + 0.05, RequestLng, radiusKm: 10)
        };
        var result = MatchingGeo.Apply([Row(1, MatchingScores.Service)], areas, RequestLat, RequestLng);

        Assert.Single(result);
        Assert.NotNull(result[0].DistanceKm);
        Assert.True(result[0].DistanceKm < 0.01);
    }

    [Fact]
    public void InactiveOrInvalidAreas_AreIgnored()
    {
        var areas = new[]
        {
            Area(1, RequestLat, RequestLng, radiusKm: 10, isActive: false),
            Area(1, RequestLat, RequestLng, radiusKm: 10, isDeleted: true),
            Area(1, RequestLat, RequestLng, radiusKm: 0),
            new ServiceAreaGeo(1, null, (decimal)RequestLng, 10, true, false),
            new ServiceAreaGeo(1, (decimal)RequestLat, null, 10, true, false)
        };

        var result = MatchingGeo.Apply([Row(1, MatchingScores.Service)], areas, RequestLat, RequestLng);
        Assert.Null(result[0].DistanceKm);
        Assert.Equal(MatchingScores.Service, result[0].Score);
    }

    [Fact]
    public void IncompleteRequestCoordinates_LeaveCandidatesUnchanged()
    {
        var candidate = Row(1, MatchingScores.Service);
        var result = MatchingGeo.Apply(
            [candidate],
            [Area(1, RequestLat, RequestLng, 10)],
            RequestLat,
            null);

        Assert.Same(candidate, result[0]);
    }

    [Fact]
    public void IndependentProvider_IsMatchable_WithoutBusinessOwnerId()
    {
        var independent = Row(42, MatchingScores.Service);
        var result = MatchingGeo.Apply(
            [independent],
            [Area(42, RequestLat, RequestLng, 8)],
            RequestLat,
            RequestLng);

        Assert.Single(result);
        Assert.Equal(42, result[0].CandidateId);
        Assert.NotNull(result[0].DistanceKm);
    }

    [Fact]
    public void ProviderWithSeparateBusinessCandidate_IsNotDuplicatedAsProvider()
    {
        var provider = Row(7, MatchingScores.Service);
        var business = new MatchingCandidateRow(
            MatchCandidateType.Business, 99, "Shop", MatchingScores.Service);
        var result = MatchingGeo.Apply(
            [provider, business],
            [Area(7, RequestLat, RequestLng, 5)],
            RequestLat,
            RequestLng);

        Assert.Equal(2, result.Count);
        Assert.Single(result, row => row.CandidateType == MatchCandidateType.Provider && row.CandidateId == 7);
        Assert.Single(result, row => row.CandidateType == MatchCandidateType.Business && row.CandidateId == 99);
        Assert.Null(result.Single(row => row.CandidateType == MatchCandidateType.Business).DistanceKm);
    }

    private static MatchingCandidateRow Row(long id, int score) =>
        new(MatchCandidateType.Provider, id, "Independent", score);

    private static ServiceAreaGeo Area(
        long ownerId,
        double lat,
        double lng,
        decimal radiusKm,
        bool isActive = true,
        bool isDeleted = false) =>
        new(ownerId, (decimal)lat, (decimal)lng, radiusKm, isActive, isDeleted);
}

public sealed class MatchingRankerTests
{
    [Fact]
    public void Rank_OrdersNearestFirst_ThenScore_AndCapsAtMaxResults()
    {
        var far = new MatchingCandidateRow(MatchCandidateType.Provider, 1, "Far", 50, true, 12);
        var near = new MatchingCandidateRow(MatchCandidateType.Provider, 2, "Near", 50, true, 3);
        var mid = new MatchingCandidateRow(MatchCandidateType.Provider, 3, "Mid", 70, true, 7);
        var noGeo = new MatchingCandidateRow(MatchCandidateType.Provider, 4, "Text", 80);

        var ranked = MatchingRanker.Rank([far, near, mid, noGeo]);

        Assert.Equal([2L, 3L, 1L, 4L], ranked.Select(r => r.CandidateId).ToArray());
        Assert.Equal(1, ranked[0].Rank);
        Assert.Equal(3, ranked[0].DistanceKm);
        Assert.Null(ranked[^1].DistanceKm);
    }

    [Fact]
    public void Rank_DeduplicatesProvider_KeepingNearest()
    {
        var first = new MatchingCandidateRow(MatchCandidateType.Provider, 1, "A", 50, true, 9);
        var closer = new MatchingCandidateRow(MatchCandidateType.Provider, 1, "A", 50, true, 2);

        var ranked = MatchingRanker.Rank([first, closer]);

        Assert.Single(ranked);
        Assert.Equal(2, ranked[0].DistanceKm);
    }

    [Fact]
    public void Rank_WithoutDistances_KeepsScoreOrder_AndMaxResults()
    {
        var rows = Enumerable.Range(1, 60)
            .Select(i => new MatchingCandidateRow(MatchCandidateType.Provider, i, $"P{i}", i))
            .ToList();

        var ranked = MatchingRanker.Rank(rows);

        Assert.Equal(MatchingScores.MaxResults, ranked.Count);
        Assert.Equal(60, ranked[0].CandidateId);
        Assert.Equal(11, ranked[^1].CandidateId);
        Assert.All(ranked, item => Assert.Null(item.DistanceKm));
    }
}

public sealed class GetRequestMatchesQueryTests
{
    [Fact]
    public void ToCriteria_RequiresBothCoordinates()
    {
        var complete = RequestWithLocation(35.7m, 51.4m);
        var completeCriteria = GetRequestMatchesQueryHandler.ToCriteria(complete);
        Assert.Equal(35.7, completeCriteria.Latitude);
        Assert.Equal(51.4, completeCriteria.Longitude);

        var incomplete = RequestWithLocation(35.7m, null);
        var incompleteCriteria = GetRequestMatchesQueryHandler.ToCriteria(incomplete);
        Assert.Null(incompleteCriteria.Latitude);
        Assert.Null(incompleteCriteria.Longitude);
    }

    [Fact]
    public void ToCriteria_KeepsCityProvinceDistrict()
    {
        var request = new Request(1, "Service", "Need").WithId(1);
        var location = new RequestLocation(1, 1, 10, 120, null, 35.7m, 51.4m)
            .Set(nameof(RequestLocation.Province), new LocationProvince("Tehran", "THR").WithId(1))
            .Set(nameof(RequestLocation.City), new LocationCity(1, "Tehran").WithId(10))
            .Set(nameof(RequestLocation.District), new LocationDistrict(10, "Saadat").WithId(120));
        request.AddLocation(location);
        var criteria = GetRequestMatchesQueryHandler.ToCriteria(request);
        Assert.Equal("tehran", criteria.Province);
        Assert.Equal("tehran", criteria.City);
        Assert.Equal("saadat", criteria.District);
    }

    [Fact]
    public async Task CancelledRequest_DoesNotQueryMatches()
    {
        var request = new Request(1, "Service", "Need").WithId(1);
        request.Cancel();
        var matching = new FakeMatchingReadRepository { ThrowIfMatchQueried = true };
        var handler = new GetRequestMatchesQueryHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            new FakeRequestRepository { Owned = request },
            matching);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new GetRequestMatchesQuery(1), CancellationToken.None));
        Assert.Equal(0, matching.ProviderMatchQueryCount);
    }

    [Fact]
    public async Task OpenRequest_WithoutCoordinates_UsesExistingScoreOrder()
    {
        var request = new Request(1, "Service", "Need").WithId(1);
        var matching = new FakeMatchingReadRepository();
        matching.ProviderMatches.Add(new MatchingCandidateRow(MatchCandidateType.Provider, 2, "B", 50));
        matching.ProviderMatches.Add(new MatchingCandidateRow(MatchCandidateType.Provider, 1, "A", 70));
        var handler = new GetRequestMatchesQueryHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            new FakeRequestRepository { Owned = request },
            matching);

        var result = await handler.Handle(new GetRequestMatchesQuery(1), CancellationToken.None);

        Assert.Equal([1L, 2L], result.Select(r => r.CandidateId).ToArray());
        Assert.All(result, item => Assert.Null(item.DistanceKm));
    }

    private static Request RequestWithLocation(decimal? lat, decimal? lng)
    {
        var request = new Request(1, "Service", "Need").WithId(1);
        request.AddLocation(new RequestLocation(1, 1, 10, 120, null, lat, lng));
        return request;
    }
}

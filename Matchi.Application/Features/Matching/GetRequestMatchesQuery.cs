using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Matching;

public sealed record GetRequestMatchesQuery(long RequestId) : IRequest<IReadOnlyList<MatchResultDto>>;

public sealed class GetRequestMatchesQueryHandler
    : IRequestHandler<GetRequestMatchesQuery, IReadOnlyList<MatchResultDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestRepository _requestRepository;
    private readonly IMatchingReadRepository _matchingReadRepository;

    public GetRequestMatchesQueryHandler(
        ICurrentUserService currentUserService,
        IRequestRepository requestRepository,
        IMatchingReadRepository matchingReadRepository)
    {
        _currentUserService = currentUserService;
        _requestRepository = requestRepository;
        _matchingReadRepository = matchingReadRepository;
    }

    public async Task<IReadOnlyList<MatchResultDto>> Handle(
        GetRequestMatchesQuery query,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Authenticated user was not found.");

        var request = await _requestRepository.GetOwnedByIdAsync(
            query.RequestId,
            userId,
            cancellationToken);

        if (request is null)
            throw new KeyNotFoundException("Request was not found.");

        if (string.Equals(request.Status, "Cancelled", StringComparison.Ordinal))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("requestId", "A cancelled request cannot be matched.")
            });
        }

        var criteria = ToCriteria(request);

        var providers = await _matchingReadRepository.FindProviderMatchesAsync(criteria, cancellationToken);
        var businesses = await _matchingReadRepository.FindBusinessMatchesAsync(criteria, cancellationToken);

        return MatchingRanker.Rank(providers.Concat(businesses));
    }

    internal static MatchingCriteria ToCriteria(Request request)
    {
        var services = request.Services.Where(s => !s.IsDeleted).ToList();
        var products = request.Products.Where(p => !p.IsDeleted).ToList();
        var location = request.Locations.OrderBy(l => l.Id).FirstOrDefault();
        var schedule = request.Schedules.OrderBy(s => s.Id).FirstOrDefault();
        var point = ReadPoint(location);

        return new MatchingCriteria
        {
            ServiceIds = services.Select(s => s.ServiceId).Distinct().ToList(),
            ProductIds = products.Where(p => p.ProductId.HasValue).Select(p => p.ProductId!.Value).Distinct().ToList(),
            ProductCategoryIds = products
                .Where(p => p.ProductCategoryId.HasValue)
                .Select(p => p.ProductCategoryId!.Value)
                .Distinct()
                .ToList(),
            ServiceAttributeIds = services
                .SelectMany(s => s.Attributes.Where(a => !a.IsDeleted))
                .Select(a => a.ServiceAttributeId)
                .Distinct()
                .ToList(),
            Province = Normalize(location?.Province?.Name),
            City = Normalize(location?.City?.Name),
            District = Normalize(location?.District?.Name),
            Latitude = point.Lat,
            Longitude = point.Lng,
            DayOfWeek = schedule is null ? null : (byte)schedule.Date.DayOfWeek,
            TimeFrom = schedule?.TimeFrom,
            TimeTo = schedule?.TimeTo,
            IsFlexible = schedule?.IsFlexible ?? false,
            RequireService = services.Count > 0,
            RequireProduct = services.Count == 0 && products.Count > 0
        };
    }

    private static (double? Lat, double? Lng) ReadPoint(RequestLocation? location)
    {
        if (location?.Lat is null || location.Lng is null)
            return (null, null);

        var lat = (double)location.Lat.Value;
        var lng = (double)location.Lng.Value;
        return GeoDistance.IsValidPoint(lat, lng) ? (lat, lng) : (null, null);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
}

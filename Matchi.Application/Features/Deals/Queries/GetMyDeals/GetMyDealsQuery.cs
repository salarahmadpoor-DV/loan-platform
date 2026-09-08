using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Deals;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Deals.Queries.GetMyDeals;

public sealed record GetMyDealsQuery : IRequest<IReadOnlyList<DealSummaryDto>>;

public sealed class GetMyDealsQueryHandler : IRequestHandler<GetMyDealsQuery, IReadOnlyList<DealSummaryDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDealRepository _dealRepository;

    public GetMyDealsQueryHandler(
        ICurrentUserService currentUserService,
        IDealRepository dealRepository)
    {
        _currentUserService = currentUserService;
        _dealRepository = dealRepository;
    }

    public async Task<IReadOnlyList<DealSummaryDto>> Handle(
        GetMyDealsQuery query,
        CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var deals = await _dealRepository.ListOwnedByUserAsync(userId, cancellationToken);
        return deals.Select(DealDtoMapper.ToSummary).ToList();
    }
}

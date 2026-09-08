using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Deals;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Deals.Queries.GetDealById;

public sealed record GetDealByIdQuery(long DealId) : IRequest<DealDetailDto?>;

public sealed class GetDealByIdQueryHandler : IRequestHandler<GetDealByIdQuery, DealDetailDto?>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDealRepository _dealRepository;

    public GetDealByIdQueryHandler(
        ICurrentUserService currentUserService,
        IDealRepository dealRepository)
    {
        _currentUserService = currentUserService;
        _dealRepository = dealRepository;
    }

    public async Task<DealDetailDto?> Handle(GetDealByIdQuery query, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var deal = await _dealRepository.GetOwnedByIdAsync(query.DealId, userId, cancellationToken);
        return deal is null ? null : DealDtoMapper.ToDetail(deal);
    }
}

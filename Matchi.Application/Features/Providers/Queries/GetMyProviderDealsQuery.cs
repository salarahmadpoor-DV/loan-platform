using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Queries;

public sealed record GetMyProviderDealsQuery : IRequest<IReadOnlyList<ProviderDealDto>>;

public sealed class GetMyProviderDealsQueryHandler
    : IRequestHandler<GetMyProviderDealsQuery, IReadOnlyList<ProviderDealDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;
    private readonly IDealRepository _deals;

    public GetMyProviderDealsQueryHandler(
        ICurrentUserService currentUserService,
        IProviderRepository providers,
        IDealRepository deals)
    {
        _currentUserService = currentUserService;
        _providers = providers;
        _deals = deals;
    }

    public async Task<IReadOnlyList<ProviderDealDto>> Handle(
        GetMyProviderDealsQuery query,
        CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var deals = await _deals.ListVisibleToProviderAsync(provider.Id, cancellationToken);

        return deals
            .Select(deal => new ProviderDealDto(
                deal.Id,
                deal.RequestId,
                deal.ProposalId,
                deal.Status,
                deal.TotalPrice,
                deal.AcceptedAt))
            .ToList();
    }
}

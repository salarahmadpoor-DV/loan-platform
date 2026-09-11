using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Providers.Queries;

public sealed record GetMyProviderProposalsQuery : IRequest<IReadOnlyList<ProviderProposalDto>>;

public sealed class GetMyProviderProposalsQueryHandler
    : IRequestHandler<GetMyProviderProposalsQuery, IReadOnlyList<ProviderProposalDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProviderRepository _providers;
    private readonly IProposalRepository _proposals;

    public GetMyProviderProposalsQueryHandler(
        ICurrentUserService currentUserService,
        IProviderRepository providers,
        IProposalRepository proposals)
    {
        _currentUserService = currentUserService;
        _providers = providers;
        _proposals = proposals;
    }

    public async Task<IReadOnlyList<ProviderProposalDto>> Handle(
        GetMyProviderProposalsQuery query,
        CancellationToken cancellationToken)
    {
        var provider = await ProviderAccess.RequireMine(_currentUserService, _providers, cancellationToken);
        var proposals = await _proposals.ListByProviderIdAsync(provider.Id, cancellationToken);

        return proposals
            .Select(proposal => new ProviderProposalDto(
                proposal.Id,
                proposal.RequestId,
                proposal.Status,
                proposal.TotalPrice,
                proposal.CreateDate,
                proposal.Deal is { IsDeleted: false } deal ? deal.Id : null))
            .ToList();
    }
}

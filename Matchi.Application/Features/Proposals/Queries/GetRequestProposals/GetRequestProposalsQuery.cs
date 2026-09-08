using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Proposals;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Proposals.Queries.GetRequestProposals;

public sealed record GetRequestProposalsQuery(long RequestId) : IRequest<IReadOnlyList<ProposalListDto>>;

public sealed class GetRequestProposalsQueryHandler
    : IRequestHandler<GetRequestProposalsQuery, IReadOnlyList<ProposalListDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProposalRepository _proposalRepository;

    public GetRequestProposalsQueryHandler(
        ICurrentUserService currentUserService,
        IProposalRepository proposalRepository)
    {
        _currentUserService = currentUserService;
        _proposalRepository = proposalRepository;
    }

    public async Task<IReadOnlyList<ProposalListDto>> Handle(
        GetRequestProposalsQuery query,
        CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var owned = await _proposalRepository.IsRequestOwnedByUserAsync(
            query.RequestId,
            userId,
            cancellationToken);

        if (!owned)
            throw new KeyNotFoundException("Request was not found.");

        var proposals = await _proposalRepository.ListOwnedRequestProposalsAsync(
            query.RequestId,
            userId,
            cancellationToken);

        return proposals.Select(ProposalDtoMapper.ToListDto).ToList();
    }
}

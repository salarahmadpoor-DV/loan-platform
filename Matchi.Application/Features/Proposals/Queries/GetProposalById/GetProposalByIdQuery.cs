using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Proposals;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Proposals.Queries.GetProposalById;

public sealed record GetProposalByIdQuery(long ProposalId) : IRequest<ProposalDetailDto?>;

public sealed class GetProposalByIdQueryHandler : IRequestHandler<GetProposalByIdQuery, ProposalDetailDto?>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProposalRepository _proposalRepository;

    public GetProposalByIdQueryHandler(
        ICurrentUserService currentUserService,
        IProposalRepository proposalRepository)
    {
        _currentUserService = currentUserService;
        _proposalRepository = proposalRepository;
    }

    public async Task<ProposalDetailDto?> Handle(GetProposalByIdQuery query, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var proposal = await _proposalRepository.GetOwnedDetailAsync(
            query.ProposalId,
            userId,
            cancellationToken);

        return proposal is null ? null : ProposalDtoMapper.ToDetailDto(proposal);
    }
}

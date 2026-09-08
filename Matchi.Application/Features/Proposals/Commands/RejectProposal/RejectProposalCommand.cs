using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Proposals.Commands.RejectProposal;

public sealed record RejectProposalCommand(long ProposalId) : IRequest<string>;

public sealed class RejectProposalCommandHandler : IRequestHandler<RejectProposalCommand, string>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProposalRepository _proposalRepository;

    public RejectProposalCommandHandler(
        ICurrentUserService currentUserService,
        IProposalRepository proposalRepository)
    {
        _currentUserService = currentUserService;
        _proposalRepository = proposalRepository;
    }

    public async Task<string> Handle(RejectProposalCommand command, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var proposal = await _proposalRepository.GetTrackedOwnedByRequestOwnerAsync(
            command.ProposalId,
            userId,
            cancellationToken);

        if (proposal is null)
            throw new KeyNotFoundException("Proposal was not found.");

        if (!string.Equals(proposal.Status, "Pending", StringComparison.Ordinal))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("proposalId", "Only a pending proposal can be rejected.")
            });
        }

        proposal.Reject();
        await _proposalRepository.UpdateAsync(cancellationToken);
        return proposal.Status;
    }
}

using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Proposals.Commands.AcceptProposal;

public sealed record AcceptProposalResult(long ProposalId, string Status, long DealId);

public sealed record AcceptProposalCommand(long ProposalId) : IRequest<AcceptProposalResult>;

public sealed class AcceptProposalCommandHandler : IRequestHandler<AcceptProposalCommand, AcceptProposalResult>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IProposalRepository _proposalRepository;
    private readonly IDealRepository _dealRepository;

    public AcceptProposalCommandHandler(
        ICurrentUserService currentUserService,
        IProposalRepository proposalRepository,
        IDealRepository dealRepository)
    {
        _currentUserService = currentUserService;
        _proposalRepository = proposalRepository;
        _dealRepository = dealRepository;
    }

    public async Task<AcceptProposalResult> Handle(
        AcceptProposalCommand command,
        CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var proposal = await _proposalRepository.GetTrackedOwnedByRequestOwnerAsync(
            command.ProposalId,
            userId,
            cancellationToken);

        if (proposal is null)
            throw new KeyNotFoundException("Proposal was not found.");

        if (!string.Equals(proposal.Request.Status, "Open", StringComparison.Ordinal))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("requestId", "A proposal can only be accepted for an open request.")
            });
        }

        if (!string.Equals(proposal.Status, "Pending", StringComparison.Ordinal))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("proposalId", "Only a pending proposal can be accepted.")
            });
        }

        proposal.Accept();

        var deal = Deal.Create(
            proposal.RequestId,
            proposal.Id,
            proposal.Request.CustomerId,
            proposal.TotalPrice);

        _dealRepository.Add(deal);

        try
        {
            await _dealRepository.SaveChangesAsync(cancellationToken);
        }
        catch (InvalidOperationException ex) when (ex.Message == "A deal already exists for this proposal.")
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("proposalId", "A deal already exists for this proposal.")
            });
        }

        return new AcceptProposalResult(proposal.Id, proposal.Status, deal.Id);
    }
}

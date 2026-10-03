using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Application.Notifications;
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
    private readonly INotificationService _notifications;
    private readonly NotificationRecipientResolver _recipients;

    public AcceptProposalCommandHandler(
        ICurrentUserService currentUserService,
        IProposalRepository proposalRepository,
        IDealRepository dealRepository,
        INotificationService notifications,
        NotificationRecipientResolver recipients)
    {
        _currentUserService = currentUserService;
        _proposalRepository = proposalRepository;
        _dealRepository = dealRepository;
        _notifications = notifications;
        _recipients = recipients;
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

        if (proposal.Request.RequestType is "Service" or "Hybrid")
        {
            ServiceExecution.CreateForDeal(
                deal,
                proposal.BusinessId,
                proposal.ProposedDate,
                proposal.ProposedTimeFrom,
                proposal.ProposedTimeTo);
        }

        _dealRepository.Add(deal);
        await _dealRepository.SaveChangesAsync(cancellationToken);

        var originator = await _recipients.ProposalOriginatorUserIdAsync(proposal, cancellationToken);
        if (originator is > 0)
        {
            await _notifications.NotifyAsync(
                originator.Value,
                NotificationCatalog.ProposalAccepted(deal.Id),
                cancellationToken);
        }

        return new AcceptProposalResult(proposal.Id, proposal.Status, deal.Id);
    }
}

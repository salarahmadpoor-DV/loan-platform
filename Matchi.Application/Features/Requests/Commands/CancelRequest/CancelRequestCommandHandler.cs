using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Notifications;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Commands.CancelRequest;

public sealed class CancelRequestCommandHandler : IRequestHandler<CancelRequestCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestRepository _requestRepository;
    private readonly IDealRepository _dealRepository;
    private readonly IProposalRepository _proposalRepository;
    private readonly INotificationService _notifications;
    private readonly NotificationRecipientResolver _recipients;

    public CancelRequestCommandHandler(
        ICurrentUserService currentUserService,
        IRequestRepository requestRepository,
        IDealRepository dealRepository,
        IProposalRepository proposalRepository,
        INotificationService notifications,
        NotificationRecipientResolver recipients)
    {
        _currentUserService = currentUserService;
        _requestRepository = requestRepository;
        _dealRepository = dealRepository;
        _proposalRepository = proposalRepository;
        _notifications = notifications;
        _recipients = recipients;
    }

    public async Task<bool> Handle(CancelRequestCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
            throw new UnauthorizedAccessException("Authenticated user was not found.");

        var request = await _requestRepository.GetOwnedByIdAsync(
            command.RequestId,
            userId.Value,
            cancellationToken);

        if (request is null)
            throw new KeyNotFoundException("Request was not found.");

        if (!request.CanBeModified())
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("requestId", "Only an open request owned by the current user can be cancelled.")
            });
        }

        if (await _dealRepository.HasActiveDealForRequestAsync(request.Id, cancellationToken))
            throw new ConflictException("This request cannot be cancelled while it has an active deal.");

        request.Cancel();
        await _requestRepository.UpdateAsync(request, cancellationToken);

        var proposals = await _proposalRepository.ListOwnedRequestProposalsAsync(
            request.Id,
            userId.Value,
            cancellationToken);
        var originators = new List<long>();
        foreach (var proposal in proposals)
        {
            if (!string.Equals(proposal.Status, "Pending", StringComparison.Ordinal))
                continue;

            var originator = await _recipients.ProposalOriginatorUserIdAsync(proposal, cancellationToken);
            if (originator is > 0)
                originators.Add(originator.Value);
        }

        await _notifications.NotifyManyAsync(
            originators,
            NotificationCatalog.RequestCancelled(request.Id),
            cancellationToken,
            excludeUserId: userId);

        return true;
    }
}

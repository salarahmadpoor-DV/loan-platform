using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Notifications;

public sealed class NotificationRecipientResolver
{
    private readonly INotificationRepository _notifications;

    public NotificationRecipientResolver(INotificationRepository notifications)
    {
        _notifications = notifications;
    }

    public async Task<long?> CustomerUserIdAsync(Request request, CancellationToken cancellationToken = default)
    {
        if (request.Customer?.UserId > 0)
            return request.Customer.UserId;

        return await _notifications.GetCustomerUserIdAsync(request.CustomerId, cancellationToken);
    }

    public async Task<long?> ProposalOriginatorUserIdAsync(
        Proposal proposal,
        CancellationToken cancellationToken = default)
    {
        if (proposal.ProviderId is > 0)
        {
            if (proposal.Provider?.UserId > 0)
                return proposal.Provider.UserId;

            return await _notifications.GetProviderUserIdAsync(proposal.ProviderId.Value, cancellationToken);
        }

        if (proposal.BusinessId is > 0)
        {
            if (proposal.Business?.OwnerUserId > 0)
                return proposal.Business.OwnerUserId;

            return await _notifications.GetBusinessOwnerUserIdAsync(proposal.BusinessId.Value, cancellationToken);
        }

        return null;
    }

    public async Task<IReadOnlyList<long>> ExecutionPartyUserIdsAsync(
        ServiceExecution execution,
        CancellationToken cancellationToken = default)
    {
        var ids = new HashSet<long>();
        var deal = execution.Deal;
        if (deal?.Request is not null)
        {
            var customerId = await CustomerUserIdAsync(deal.Request, cancellationToken);
            if (customerId is > 0)
                ids.Add(customerId.Value);

            var originator = await ProposalOriginatorUserIdAsync(deal.Proposal, cancellationToken);
            if (originator is > 0)
                ids.Add(originator.Value);
        }

        if (execution.BusinessId is > 0)
        {
            var owner = execution.Business?.OwnerUserId > 0
                ? execution.Business.OwnerUserId
                : await _notifications.GetBusinessOwnerUserIdAsync(execution.BusinessId.Value, cancellationToken);
            if (owner is > 0)
                ids.Add(owner.Value);
        }

        foreach (var assignment in execution.Assignments)
        {
            if (!string.Equals(assignment.Status, "Assigned", StringComparison.Ordinal))
                continue;

            if (assignment.Provider?.UserId > 0)
            {
                ids.Add(assignment.Provider.UserId);
                continue;
            }

            var providerUserId = await _notifications.GetProviderUserIdAsync(assignment.ProviderId, cancellationToken);
            if (providerUserId is > 0)
                ids.Add(providerUserId.Value);
        }

        return ids.ToList();
    }
}

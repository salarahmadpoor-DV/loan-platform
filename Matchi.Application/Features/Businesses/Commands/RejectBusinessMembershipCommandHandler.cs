using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Application.Notifications;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed class RejectBusinessMembershipCommandHandler : IRequestHandler<RejectBusinessMembershipCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;
    private readonly INotificationService _notifications;

    public RejectBusinessMembershipCommandHandler(
        ICurrentUserService currentUserService,
        IBusinessRepository businesses,
        INotificationService notifications)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
        _notifications = notifications;
    }

    public async Task<bool> Handle(RejectBusinessMembershipCommand request, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var membership = await _businesses.GetMembershipByIdAsync(request.BusinessProviderId, cancellationToken);
        if (membership is null || membership.Provider.UserId != userId)
            return false;

        if (!string.Equals(membership.Status, "Pending", StringComparison.OrdinalIgnoreCase))
            return false;

        membership.Reject();
        await _businesses.UpdateAsync(cancellationToken);
        await _notifications.NotifyAsync(
            membership.Business.OwnerUserId,
            NotificationCatalog.InvitationRejected(membership.Id),
            cancellationToken);
        return true;
    }
}

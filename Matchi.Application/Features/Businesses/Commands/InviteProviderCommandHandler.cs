using Matchi.Application.Features.Providers;
using Matchi.Application.Notifications;
using Matchi.Domain.Interfaces;
using MediatR;
using Matchi.Application.Common.Interfaces;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed class InviteProviderCommandHandler : IRequestHandler<InviteProviderCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;
    private readonly IUserRepository _users;
    private readonly IProviderRepository _providers;
    private readonly INotificationService _notifications;

    public InviteProviderCommandHandler(
        ICurrentUserService currentUserService,
        IBusinessRepository businesses,
        IUserRepository users,
        IProviderRepository providers,
        INotificationService notifications)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
        _users = users;
        _providers = providers;
        _notifications = notifications;
    }

    public async Task<long> Handle(InviteProviderCommand request, CancellationToken cancellationToken)
    {
        var providerId = await ResolveProviderIdAsync(request, cancellationToken);
        var handler = new AddBusinessProviderCommandHandler(_currentUserService, _businesses);
        var membershipId = await handler.Handle(
            new AddBusinessProviderCommand(request.BusinessId, providerId, request.Role, "Pending"),
            cancellationToken);

        var provider = await _providers.GetByIdAsync(providerId, cancellationToken);
        if (provider is not null)
        {
            await _notifications.NotifyAsync(
                provider.UserId,
                NotificationCatalog.InvitationReceived(membershipId),
                cancellationToken);
        }

        return membershipId;
    }

    private async Task<long> ResolveProviderIdAsync(
        InviteProviderCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ProviderId is > 0)
            return request.ProviderId.Value;

        var mobile = request.Mobile?.Trim() ?? string.Empty;
        var user = await _users.GetByMobileAsync(mobile, cancellationToken);
        if (user is null)
            throw Fail.Validation("mobile", "No provider is registered with this mobile number.");

        var provider = await _providers.GetByUserIdAsync(user.Id, cancellationToken);
        if (provider is null)
            throw Fail.Validation("mobile", "This user is not registered as a provider.");

        return provider.Id;
    }
}

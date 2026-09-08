using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed class AcceptBusinessMembershipCommandHandler : IRequestHandler<AcceptBusinessMembershipCommand, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public AcceptBusinessMembershipCommandHandler(
        ICurrentUserService currentUserService,
        IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<bool> Handle(AcceptBusinessMembershipCommand request, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var membership = await _businesses.GetMembershipByIdAsync(request.BusinessProviderId, cancellationToken);
        if (membership is null || membership.Provider.UserId != userId)
            return false;

        membership.UpdateMembership(membership.Role, "Active");
        await _businesses.UpdateAsync(cancellationToken);
        return true;
    }
}

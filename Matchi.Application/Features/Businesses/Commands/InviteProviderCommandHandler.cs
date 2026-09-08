using MediatR;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed class InviteProviderCommandHandler : IRequestHandler<InviteProviderCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBusinessRepository _businesses;

    public InviteProviderCommandHandler(
        ICurrentUserService currentUserService,
        IBusinessRepository businesses)
    {
        _currentUserService = currentUserService;
        _businesses = businesses;
    }

    public async Task<long> Handle(InviteProviderCommand request, CancellationToken cancellationToken)
    {
        var handler = new AddBusinessProviderCommandHandler(_currentUserService, _businesses);
        return await handler.Handle(
            new AddBusinessProviderCommand(request.BusinessId, request.ProviderId, request.Role, "Pending"),
            cancellationToken);
    }
}

using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed class AcceptBusinessMembershipCommandHandler : IRequestHandler<AcceptBusinessMembershipCommand, bool>
{
    public Task<bool> Handle(
        AcceptBusinessMembershipCommand request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
}

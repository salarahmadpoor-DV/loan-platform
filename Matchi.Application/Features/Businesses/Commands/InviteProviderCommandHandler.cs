using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed class InviteProviderCommandHandler : IRequestHandler<InviteProviderCommand, long>
{
    public Task<long> Handle(
        InviteProviderCommand request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(1L);
    }
}

using MediatR;

namespace Matchi.Application.Features.Providers.Commands;

public sealed class CreateProviderCommandHandler : IRequestHandler<CreateProviderCommand, long>
{
    public Task<long> Handle(
        CreateProviderCommand request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(1L);
    }
}

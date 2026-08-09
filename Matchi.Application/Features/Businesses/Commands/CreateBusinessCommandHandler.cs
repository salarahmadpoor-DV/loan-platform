using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommand, long>
{
    public Task<long> Handle(
        CreateBusinessCommand request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(1L);
    }
}

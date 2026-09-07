using MediatR;

namespace Matchi.Application.Features.Introductions.Commands;

public sealed class CreateIntroductionCommandHandler : IRequestHandler<CreateIntroductionCommand, long>
{
    public Task<long> Handle(CreateIntroductionCommand request, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException(
            "Introductions are not part of the Matchi baseline model. Use proposals instead.");
    }
}

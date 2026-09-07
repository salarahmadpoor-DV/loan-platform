using MediatR;

namespace Matchi.Application.Features.Introductions.Commands.ConfirmIntroduction;

public sealed class ConfirmIntroductionCommandHandler : IRequestHandler<ConfirmIntroductionCommand, bool>
{
    public Task<bool> Handle(ConfirmIntroductionCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(false);
    }
}

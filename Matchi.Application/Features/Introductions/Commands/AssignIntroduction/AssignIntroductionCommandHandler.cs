using MediatR;

namespace Matchi.Application.Features.Introductions.Commands.AssignIntroduction;

public sealed class AssignIntroductionCommandHandler : IRequestHandler<AssignIntroductionCommand, bool>
{
    public Task<bool> Handle(AssignIntroductionCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(false);
    }
}

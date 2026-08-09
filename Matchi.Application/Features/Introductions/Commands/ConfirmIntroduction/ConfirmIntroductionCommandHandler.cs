using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Introductions.Commands.ConfirmIntroduction;

public sealed class ConfirmIntroductionCommandHandler : IRequestHandler<ConfirmIntroductionCommand, bool>
{
    private readonly IIntroductionRepository _introductionRepository;

    public ConfirmIntroductionCommandHandler(IIntroductionRepository introductionRepository)
    {
        _introductionRepository = introductionRepository;
    }

    public async Task<bool> Handle(
        ConfirmIntroductionCommand request,
        CancellationToken cancellationToken)
    {
        var introduction = await _introductionRepository.GetByIdAsync(request.IntroductionId, cancellationToken);
        if (introduction is null)
            return false;

        introduction.Confirm(request.Status);
        await _introductionRepository.UpdateAsync(introduction, cancellationToken);
        return true;
    }
}

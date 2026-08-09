using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Introductions.Commands.AssignIntroduction;

public sealed class AssignIntroductionCommandHandler : IRequestHandler<AssignIntroductionCommand, bool>
{
    private readonly IIntroductionRepository _introductionRepository;

    public AssignIntroductionCommandHandler(IIntroductionRepository introductionRepository)
    {
        _introductionRepository = introductionRepository;
    }

    public async Task<bool> Handle(
        AssignIntroductionCommand request,
        CancellationToken cancellationToken)
    {
        var introduction = await _introductionRepository.GetByIdAsync(request.IntroductionId, cancellationToken);
        if (introduction is null)
            return false;

        introduction.AssignProvider(request.ProviderId);
        await _introductionRepository.UpdateAsync(introduction, cancellationToken);
        return true;
    }
}

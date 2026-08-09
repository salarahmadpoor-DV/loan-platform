using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Introductions.Commands;

public sealed class CreateIntroductionCommandHandler : IRequestHandler<CreateIntroductionCommand, long>
{
    private readonly IIntroductionRepository _introductionRepository;

    public CreateIntroductionCommandHandler(IIntroductionRepository introductionRepository)
    {
        _introductionRepository = introductionRepository;
    }

    public async Task<long> Handle(
        CreateIntroductionCommand request,
        CancellationToken cancellationToken)
    {
        var introduction = new Introduction(request.RequestId, request.TargetType, request.TargetId, request.Source);
        return await _introductionRepository.AddAsync(introduction, cancellationToken);
    }
}

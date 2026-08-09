using MediatR;

namespace Matchi.Application.Features.Matching.Commands;

public sealed class MatchRequestCommandHandler : IRequestHandler<MatchRequestCommand, MatchResultDto>
{
    public Task<MatchResultDto> Handle(
        MatchRequestCommand request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new MatchResultDto(
            Array.Empty<MatchCandidateDto>(),
            Array.Empty<MatchCandidateDto>()));
    }
}

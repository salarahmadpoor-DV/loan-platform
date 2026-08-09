using MediatR;
using Matchi.Application.Features.Matching.Commands;

namespace Matchi.Application.Features.Matching.Queries;

public sealed class GetMatchesQueryHandler : IRequestHandler<GetMatchesQuery, IEnumerable<MatchCandidateDto>>
{
    public Task<IEnumerable<MatchCandidateDto>> Handle(
        GetMatchesQuery request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult((IEnumerable<MatchCandidateDto>)Array.Empty<MatchCandidateDto>());
    }
}

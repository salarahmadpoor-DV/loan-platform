using MediatR;
using Matchi.Application.Features.Matching.Commands;

namespace Matchi.Application.Features.Matching.Queries;

public sealed record GetMatchesQuery(Guid RequestId) : IRequest<IEnumerable<MatchCandidateDto>>;

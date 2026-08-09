using MediatR;

namespace Matchi.Application.Features.Introductions.Queries.GetIntroductionsByRequest;

public sealed record GetIntroductionsByRequestQuery(long RequestId) : IRequest<IEnumerable<IntroductionSummaryDto>>;

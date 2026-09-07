using MediatR;

namespace Matchi.Application.Features.Introductions.Queries.GetIntroductionsByRequest;

public sealed class GetIntroductionsByRequestQueryHandler : IRequestHandler<GetIntroductionsByRequestQuery, IEnumerable<IntroductionSummaryDto>>
{
    public Task<IEnumerable<IntroductionSummaryDto>> Handle(
        GetIntroductionsByRequestQuery request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Enumerable.Empty<IntroductionSummaryDto>());
    }
}

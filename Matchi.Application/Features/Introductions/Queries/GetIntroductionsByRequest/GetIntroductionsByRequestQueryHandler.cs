using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Introductions.Queries.GetIntroductionsByRequest;

public sealed class GetIntroductionsByRequestQueryHandler : IRequestHandler<GetIntroductionsByRequestQuery, IEnumerable<IntroductionSummaryDto>>
{
    private readonly IIntroductionRepository _introductionRepository;

    public GetIntroductionsByRequestQueryHandler(IIntroductionRepository introductionRepository)
    {
        _introductionRepository = introductionRepository;
    }

    public async Task<IEnumerable<IntroductionSummaryDto>> Handle(
        GetIntroductionsByRequestQuery request,
        CancellationToken cancellationToken)
    {
        var introductions = await _introductionRepository.GetByRequestIdAsync(request.RequestId, cancellationToken);

        return introductions.Select(i => new IntroductionSummaryDto(
            i.Id,
            i.ServiceRequestId,
            i.TargetType,
            i.TargetId,
            i.Status,
            i.AssignedProviderId));
    }
}

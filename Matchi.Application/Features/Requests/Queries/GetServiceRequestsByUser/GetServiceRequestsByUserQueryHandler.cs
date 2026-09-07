using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Queries.GetServiceRequestsByUser;

public sealed class GetServiceRequestsByUserQueryHandler : IRequestHandler<GetServiceRequestsByUserQuery, IEnumerable<ServiceRequestSummaryDto>>
{
    private readonly IRequestRepository _requestRepository;

    public GetServiceRequestsByUserQueryHandler(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<IEnumerable<ServiceRequestSummaryDto>> Handle(
        GetServiceRequestsByUserQuery request,
        CancellationToken cancellationToken)
    {
        var requests = await _requestRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return requests.Select(r => new ServiceRequestSummaryDto(
            r.Id,
            r.Title,
            r.Status,
            r.Services.OrderBy(s => s.DisplayOrder).FirstOrDefault()?.ServiceId ?? 0,
            r.CreateDate));
    }
}

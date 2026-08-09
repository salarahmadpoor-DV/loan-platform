using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Queries.GetServiceRequestsByUser;

public sealed class GetServiceRequestsByUserQueryHandler : IRequestHandler<GetServiceRequestsByUserQuery, IEnumerable<ServiceRequestSummaryDto>>
{
    private readonly IServiceRequestRepository _serviceRequestRepository;

    public GetServiceRequestsByUserQueryHandler(IServiceRequestRepository serviceRequestRepository)
    {
        _serviceRequestRepository = serviceRequestRepository;
    }

    public async Task<IEnumerable<ServiceRequestSummaryDto>> Handle(
        GetServiceRequestsByUserQuery request,
        CancellationToken cancellationToken)
    {
        var requests = await _serviceRequestRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return requests.Select(r => new ServiceRequestSummaryDto(
            r.Id,
            r.Title,
            r.Status,
            r.ServiceId,
            r.CreateDate));
    }
}

using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Queries.GetServiceRequestById;

public sealed class GetServiceRequestByIdQueryHandler : IRequestHandler<GetServiceRequestByIdQuery, ServiceRequestDto?>
{
    private readonly IServiceRequestRepository _serviceRequestRepository;

    public GetServiceRequestByIdQueryHandler(IServiceRequestRepository serviceRequestRepository)
    {
        _serviceRequestRepository = serviceRequestRepository;
    }

    public async Task<ServiceRequestDto?> Handle(
        GetServiceRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _serviceRequestRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (entity is null)
            return null;

        return new ServiceRequestDto(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.ServiceId,
            entity.UserId,
            entity.Lat,
            entity.Lng,
            entity.Status,
            entity.Answers?.Select(a => new ServiceRequestAnswerDto(a.ServiceQuestionId, a.SelectedOptionId, a.Text)).ToList() ?? new List<ServiceRequestAnswerDto>());
    }
}

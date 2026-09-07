using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Queries.GetServiceRequestById;

public sealed class GetServiceRequestByIdQueryHandler : IRequestHandler<GetServiceRequestByIdQuery, ServiceRequestDto?>
{
    private readonly IRequestRepository _requestRepository;

    public GetServiceRequestByIdQueryHandler(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<ServiceRequestDto?> Handle(
        GetServiceRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (entity is null)
            return null;

        var service = entity.Services.OrderBy(s => s.DisplayOrder).FirstOrDefault();
        var location = entity.Locations.FirstOrDefault();
        var answers = (service?.Attributes ?? Enumerable.Empty<Matchi.Domain.Entities.RequestServiceAttribute>())
            .Select(a => new ServiceRequestAnswerDto(a.ServiceAttributeId, null, a.Value))
            .ToList();

        return new ServiceRequestDto(
            entity.Id,
            entity.Title,
            entity.Description,
            service?.ServiceId ?? 0,
            entity.Customer.UserId,
            location?.Lat.HasValue == true ? (double)location.Lat.Value : null,
            location?.Lng.HasValue == true ? (double)location.Lng.Value : null,
            entity.Status,
            answers);
    }
}

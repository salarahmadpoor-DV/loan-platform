using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Services.Queries.GetServiceById;

public sealed class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ServiceDetailDto?>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServiceByIdQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<ServiceDetailDto?> Handle(
        GetServiceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetServiceByIdAsync(request.ServiceId, cancellationToken);
        if (service is null)
            return null;

        return new ServiceDetailDto(
            service.Id,
            service.Name,
            service.CategoryId,
            service.Questions?.Count ?? 0);
    }
}

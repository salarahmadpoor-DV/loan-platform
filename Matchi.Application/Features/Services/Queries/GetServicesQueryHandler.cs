using MediatR;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Features.Services.Queries;

public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, IEnumerable<ServiceDto>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServicesQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<IEnumerable<ServiceDto>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _serviceRepository.GetServicesAsync(request.CategoryId, cancellationToken);
        return services.Select(s => new ServiceDto(s.Id, s.Name, s.CategoryId));
    }
}
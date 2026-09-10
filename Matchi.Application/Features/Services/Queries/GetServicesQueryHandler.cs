using Matchi.Application.Common;
using Matchi.Domain.Interfaces;
using MediatR;

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
        var paging = ListPaging.Normalize(request.Page, request.PageSize);
        var services = await _serviceRepository.GetServicesAsync(
            request.CategoryId,
            request.Query,
            paging.Skip,
            paging.PageSize,
            cancellationToken);
        return services.Select(s => new ServiceDto(s.Id, s.Name, s.CategoryId));
    }
}
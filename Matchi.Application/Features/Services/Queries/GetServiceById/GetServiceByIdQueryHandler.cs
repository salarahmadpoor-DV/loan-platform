using Matchi.Application.Features.Catalog;
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

        var attributes = (service.Attributes ?? Array.Empty<Domain.Entities.ServiceAttribute>())
            .OrderBy(a => a.DisplayOrder)
            .ThenBy(a => a.Id)
            .Select(a => new CatalogAttributeDto(
                a.Id,
                a.Name,
                a.Code,
                a.DataType,
                a.IsRequired,
                a.DisplayOrder,
                a.Options
                    .OrderBy(o => o.DisplayOrder)
                    .ThenBy(o => o.Id)
                    .Select(o => new CatalogAttributeOptionDto(o.Id, o.Value, o.DisplayName, o.DisplayOrder))
                    .ToList()))
            .ToList();

        return new ServiceDetailDto(
            service.Id,
            service.Name,
            service.CategoryId,
            attributes.Count,
            attributes);
    }
}

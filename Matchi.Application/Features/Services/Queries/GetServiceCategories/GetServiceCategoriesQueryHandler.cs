using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Services.Queries.GetServiceCategories;

public sealed class GetServiceCategoriesQueryHandler : IRequestHandler<GetServiceCategoriesQuery, IEnumerable<ServiceCategoryDto>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServiceCategoriesQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<IEnumerable<ServiceCategoryDto>> Handle(
        GetServiceCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _serviceRepository.GetCategoriesAsync(cancellationToken);

        return categories.Select(x => new ServiceCategoryDto(x.Id, x.Name));
    }
}

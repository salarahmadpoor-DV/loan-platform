using MediatR;

namespace Matchi.Application.Features.Services.Queries.GetServiceCategories;

public sealed record GetServiceCategoriesQuery : IRequest<IEnumerable<ServiceCategoryDto>>;

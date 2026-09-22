using Matchi.Application.Common;
using Matchi.Application.Features.Catalog;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Products.Queries;

public record GetProductCategoriesQuery : IRequest<IReadOnlyList<ProductCategoryDto>>;

public record ProductCategoryDto(long Id, string Name, long? ParentId, int DisplayOrder);

public record GetProductsQuery(long? CategoryId, string? Query, int Page, int PageSize)
    : IRequest<IReadOnlyList<ProductDto>>;

public record ProductDto(long Id, string Name, long CategoryId);

public record GetProductByIdQuery(long ProductId) : IRequest<ProductDto?>;

public record GetProductCategoryAttributesQuery(long ProductCategoryId)
    : IRequest<IReadOnlyList<CatalogAttributeDto>>;

public sealed class GetProductCategoriesQueryHandler
    : IRequestHandler<GetProductCategoriesQuery, IReadOnlyList<ProductCategoryDto>>
{
    private readonly IProductCatalogRepository _products;

    public GetProductCategoriesQueryHandler(IProductCatalogRepository products)
    {
        _products = products;
    }

    public async Task<IReadOnlyList<ProductCategoryDto>> Handle(
        GetProductCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _products.GetCategoriesAsync(cancellationToken);
        return categories
            .Select(c => new ProductCategoryDto(c.Id, c.Name, c.ParentId, c.DisplayOrder))
            .ToList();
    }
}

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IProductCatalogRepository _products;

    public GetProductsQueryHandler(IProductCatalogRepository products)
    {
        _products = products;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var paging = ListPaging.Normalize(request.Page, request.PageSize);
        var products = await _products.GetProductsAsync(
            request.CategoryId,
            request.Query,
            paging.Skip,
            paging.PageSize,
            cancellationToken);

        return products
            .Select(p => new ProductDto(p.Id, p.Name, p.CategoryId))
            .ToList();
    }
}

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductCatalogRepository _products;

    public GetProductByIdQueryHandler(IProductCatalogRepository products)
    {
        _products = products;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _products.GetProductByIdAsync(request.ProductId, cancellationToken);
        return product is null ? null : new ProductDto(product.Id, product.Name, product.CategoryId);
    }
}

public sealed class GetProductCategoryAttributesQueryHandler
    : IRequestHandler<GetProductCategoryAttributesQuery, IReadOnlyList<CatalogAttributeDto>>
{
    private readonly IProductCatalogRepository _products;

    public GetProductCategoryAttributesQueryHandler(IProductCatalogRepository products)
    {
        _products = products;
    }

    public async Task<IReadOnlyList<CatalogAttributeDto>> Handle(
        GetProductCategoryAttributesQuery request,
        CancellationToken cancellationToken)
    {
        var attributes = await _products.GetAttributesByCategoryIdAsync(
            request.ProductCategoryId,
            cancellationToken);

        return attributes
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
    }
}

using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IProductCatalogRepository
{
    Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetProductsAsync(
        long? categoryId,
        string? query,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<Product?> GetProductByIdAsync(long productId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductAttribute>> GetAttributesByCategoryIdAsync(
        long productCategoryId,
        CancellationToken cancellationToken = default);
}

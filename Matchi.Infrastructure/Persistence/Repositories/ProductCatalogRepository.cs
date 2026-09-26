using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class ProductCatalogRepository : IProductCatalogRepository
{
    private readonly MatchiDbContext _context;

    public ProductCatalogRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.ProductCategories
            .AsNoTracking()
            .Where(c => !c.IsDeleted && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(
        long? categoryId,
        string? query,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var q = _context.Products.AsNoTracking().Where(p => !p.IsDeleted && p.IsActive);

        if (categoryId.HasValue)
            q = q.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            q = q.Where(p => p.Name.Contains(term));
        }

        return await q
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetProductByIdAsync(long productId, CancellationToken cancellationToken = default)
    {
        return _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted && p.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductAttribute>> GetAttributesByCategoryIdAsync(
        long productCategoryId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProductAttributes
            .AsNoTracking()
            .Include(a => a.Options.Where(o => !o.IsDeleted && o.IsActive))
            .Where(a => a.ProductCategoryId == productCategoryId && !a.IsDeleted && a.IsActive)
            .OrderBy(a => a.DisplayOrder)
            .ThenBy(a => a.Id)
            .ToListAsync(cancellationToken);
    }
}

using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class RequestRepository : IRequestRepository
{
    private readonly MatchiDbContext _context;

    public RequestRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<Customer> GetOrCreateCustomerAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted, cancellationToken);

        if (customer is not null)
            return customer;

        customer = new Customer(userId);
        await _context.Customers.AddAsync(customer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public Task<bool> ServiceExistsAsync(long serviceId, CancellationToken cancellationToken = default)
    {
        return _context.Services.AnyAsync(s => s.Id == serviceId && !s.IsDeleted && s.IsActive, cancellationToken);
    }

    public Task<ServiceAttribute?> GetServiceAttributeAsync(
        long serviceId,
        long serviceAttributeId,
        CancellationToken cancellationToken = default)
    {
        return _context.ServiceAttributes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                a => a.Id == serviceAttributeId
                     && a.ServiceId == serviceId
                     && !a.IsDeleted
                     && a.IsActive,
                cancellationToken);
    }

    public Task<Product?> GetProductAsync(long productId, CancellationToken cancellationToken = default)
    {
        return _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted && p.IsActive, cancellationToken);
    }

    public Task<bool> ProductCategoryExistsAsync(long productCategoryId, CancellationToken cancellationToken = default)
    {
        return _context.ProductCategories
            .AnyAsync(c => c.Id == productCategoryId && !c.IsDeleted, cancellationToken);
    }

    public Task<ProductAttribute?> GetProductAttributeAsync(
        long productCategoryId,
        long productAttributeId,
        CancellationToken cancellationToken = default)
    {
        return _context.ProductAttributes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                a => a.Id == productAttributeId
                     && a.ProductCategoryId == productCategoryId
                     && !a.IsDeleted
                     && a.IsActive,
                cancellationToken);
    }

    public async Task AddAsync(Request request, CancellationToken cancellationToken = default)
    {
        await _context.Requests.AddAsync(request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Request request, CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void RemoveLocationsAndSchedules(Request request)
    {
        _context.RequestLocations.RemoveRange(request.Locations);
        _context.RequestSchedules.RemoveRange(request.Schedules);
        request.Locations.Clear();
        request.Schedules.Clear();
    }

    public Task<Request?> GetByIdAsync(
        long requestId,
        CancellationToken cancellationToken = default)
    {
        return _context.Requests
            .FirstOrDefaultAsync(r => r.Id == requestId && !r.IsDeleted, cancellationToken);
    }

    public Task<Request?> GetOwnedByIdAsync(
        long requestId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return QueryAggregate()
            .FirstOrDefaultAsync(
                r => r.Id == requestId && r.Customer.UserId == userId && !r.IsDeleted,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Request>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await QueryAggregate()
            .Where(r => r.Customer.UserId == userId && !r.IsDeleted)
            .OrderByDescending(r => r.CreateDate)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Request> QueryAggregate()
    {
        return _context.Requests
            .Include(r => r.Customer)
            .Include(r => r.Locations)
            .Include(r => r.Schedules)
            .Include(r => r.Services.Where(s => !s.IsDeleted))
                .ThenInclude(s => s.Attributes.Where(a => !a.IsDeleted))
            .Include(r => r.Products.Where(p => !p.IsDeleted))
                .ThenInclude(p => p.Attributes.Where(a => !a.IsDeleted));
    }
}

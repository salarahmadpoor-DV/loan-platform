using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly MatchiDbContext _context;

    public ServiceRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ServiceCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ServiceCategories.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Service>> GetServicesAsync(long? categoryId = null, CancellationToken cancellationToken = default)
    {
        var q = _context.Services.AsNoTracking().AsQueryable();
        if (categoryId.HasValue)
            q = q.Where(s => s.CategoryId == categoryId.Value);
        return await q.ToListAsync(cancellationToken);
    }

    public async Task<Service?> GetServiceByIdAsync(long serviceId, CancellationToken cancellationToken = default)
    {
        return await _context.Services.Include(s => s.Questions).ThenInclude(q => q.Options).AsNoTracking().FirstOrDefaultAsync(s => s.Id == serviceId, cancellationToken);
    }
}
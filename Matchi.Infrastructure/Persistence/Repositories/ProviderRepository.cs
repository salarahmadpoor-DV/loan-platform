using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public class ProviderRepository : IProviderRepository
{
    private readonly MatchiDbContext _context;

    public ProviderRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<Provider?> GetByIdAsync(long providerId, CancellationToken cancellationToken = default)
    {
        return await _context.Providers
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == providerId, cancellationToken);
    }

    public async Task<IEnumerable<Provider>> GetProvidersByServiceIdAsync(long serviceId, CancellationToken cancellationToken = default)
    {
        var query = _context.ProviderServices
            .Where(ps => ps.ServiceId == serviceId)
            .Join(_context.Providers, ps => ps.ProviderId, p => p.Id, (ps, p) => p)
            .Where(p => p.IsActive)
            .AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }
}
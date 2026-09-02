
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class BusinessRepository : IBusinessRepository
{
    private readonly MatchiDbContext _context;

    public BusinessRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Business business,
        CancellationToken cancellationToken = default)
    {
        await _context.Businesses.AddAsync(
            business,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}


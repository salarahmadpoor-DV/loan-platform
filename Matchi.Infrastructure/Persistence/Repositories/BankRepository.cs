using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Matchi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public class BankRepository : IBankRepository
{
    private readonly MatchiDbContext _context;

    public BankRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<List<Bank>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Banks
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<Bank?> GetBySlugAsync(
    string slug,
    CancellationToken cancellationToken = default)
{
    return await _context.Banks
        .Include(x => x.Questions)
        .FirstOrDefaultAsync(
            x => x.Slug == slug &&
                 !x.IsDeleted,
            cancellationToken);
}
    
}
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Matchi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public class IntroductionRepository : IIntroductionRepository
{
    private readonly MatchiDbContext _context;

    public IntroductionRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<long> AddAsync(Introduction introduction, CancellationToken cancellationToken = default)
    {
        await _context.Introductions.AddAsync(introduction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return introduction.Id;
    }

    public async Task<IEnumerable<Introduction>> GetByRequestIdAsync(long requestId, CancellationToken cancellationToken = default)
    {
        return await _context.Introductions
            .Where(i => i.ServiceRequestId == requestId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Introduction?> GetByIdAsync(long introductionId, CancellationToken cancellationToken = default)
    {
        return await _context.Introductions
            .FirstOrDefaultAsync(i => i.Id == introductionId, cancellationToken);
    }

    public async Task UpdateAsync(Introduction introduction, CancellationToken cancellationToken = default)
    {
        _context.Introductions.Update(introduction);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

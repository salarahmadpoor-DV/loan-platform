using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class DealRepository : IDealRepository
{
    private readonly MatchiDbContext _context;

    public DealRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public void Add(Deal deal)
    {
        _context.Deals.Add(deal);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicateProposalDeal(ex))
        {
            throw new InvalidOperationException("A deal already exists for this proposal.", ex);
        }
    }

    public async Task<IReadOnlyList<Deal>> ListOwnedByUserAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Deals
            .AsNoTracking()
            .Where(d =>
                !d.IsDeleted
                && !d.Request.IsDeleted
                && d.Request.Customer.UserId == userId)
            .OrderByDescending(d => d.AcceptedAt)
            .ThenByDescending(d => d.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Deal?> GetOwnedByIdAsync(
        long dealId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Deals
            .AsNoTracking()
            .FirstOrDefaultAsync(
                d => d.Id == dealId
                     && !d.IsDeleted
                     && !d.Request.IsDeleted
                     && d.Request.Customer.UserId == userId,
                cancellationToken);
    }

    private static bool IsDuplicateProposalDeal(DbUpdateException exception)
    {
        var sql = exception.InnerException as SqlException
                  ?? exception.InnerException?.InnerException as SqlException;
        if (sql is null)
            return false;

        if (sql.Number is not (2601 or 2627))
            return false;

        return sql.Message.Contains("UX_Deals_ProposalId", StringComparison.OrdinalIgnoreCase)
               || sql.Message.Contains("Deals", StringComparison.OrdinalIgnoreCase)
                  && sql.Message.Contains("ProposalId", StringComparison.OrdinalIgnoreCase);
    }
}

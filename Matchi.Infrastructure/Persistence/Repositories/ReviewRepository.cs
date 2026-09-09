using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class ReviewRepository : IReviewRepository
{
    private readonly MatchiDbContext _context;

    public ReviewRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public Task<Deal?> GetCustomerDealGraphAsync(
        long dealId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Deals
            .Include(d => d.Request)
                .ThenInclude(r => r.Customer)
            .Include(d => d.Proposal)
            .Include(d => d.ServiceExecutions)
                .ThenInclude(e => e.Assignments)
            .FirstOrDefaultAsync(
                d => d.Id == dealId
                     && !d.IsDeleted
                     && !d.Request.IsDeleted
                     && d.Request.Customer.UserId == userId,
                cancellationToken);
    }

    public Task<bool> ExistsActiveTargetAsync(
        long dealId,
        long customerId,
        long? businessId,
        long? providerId,
        CancellationToken cancellationToken = default)
    {
        return _context.Reviews.AnyAsync(
            r => r.DealId == dealId
                 && r.CustomerId == customerId
                 && !r.IsDeleted
                 && r.BusinessId == businessId
                 && r.ProviderId == providerId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Review>> ListByProviderAsync(
        long providerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => r.ProviderId == providerId && !r.IsDeleted)
            .OrderByDescending(r => r.CreateDate)
            .ThenByDescending(r => r.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Review>> ListByBusinessAsync(
        long businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => r.BusinessId == businessId && !r.IsDeleted)
            .OrderByDescending(r => r.CreateDate)
            .ThenByDescending(r => r.Id)
            .ToListAsync(cancellationToken);
    }

    public void Add(Review review)
    {
        _context.Reviews.Add(review);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicateReview(ex))
        {
            throw new InvalidOperationException("A review already exists for this target.", ex);
        }
    }

    private static bool IsDuplicateReview(DbUpdateException exception)
    {
        var sql = exception.InnerException as SqlException
                  ?? exception.InnerException?.InnerException as SqlException;
        if (sql is null)
            return false;

        if (sql.Number is not (2601 or 2627))
            return false;

        return sql.Message.Contains("UX_Reviews_Deal_Customer", StringComparison.OrdinalIgnoreCase);
    }
}

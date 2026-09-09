using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class ServiceExecutionRepository : IServiceExecutionRepository
{
    private readonly MatchiDbContext _context;

    public ServiceExecutionRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public Task<Deal?> GetActiveDealGraphAsync(long dealId, CancellationToken cancellationToken = default)
    {
        return _context.Deals
            .Include(d => d.Request)
                .ThenInclude(r => r.Customer)
            .Include(d => d.Proposal)
                .ThenInclude(p => p.Provider)
            .Include(d => d.Proposal)
                .ThenInclude(p => p.Business)
            .FirstOrDefaultAsync(
                d => d.Id == dealId && !d.IsDeleted && !d.Request.IsDeleted,
                cancellationToken);
    }

    public Task<bool> ExistsForDealAsync(long dealId, CancellationToken cancellationToken = default)
    {
        return _context.ServiceExecutions.AnyAsync(e => e.DealId == dealId, cancellationToken);
    }

    public Task<bool> IsDealVisibleAsync(long dealId, long userId, CancellationToken cancellationToken = default)
    {
        return _context.Deals.AnyAsync(
            d => d.Id == dealId
                 && !d.IsDeleted
                 && !d.Request.IsDeleted
                 && (d.Request.Customer.UserId == userId
                     || (d.Proposal.ProviderId != null && d.Proposal.Provider!.UserId == userId)
                     || (d.Proposal.BusinessId != null && d.Proposal.Business!.OwnerUserId == userId)
                     || d.ServiceExecutions.Any(e => e.Assignments.Any(a => a.Provider.UserId == userId))),
            cancellationToken);
    }

    public void Add(ServiceExecution execution)
    {
        _context.ServiceExecutions.Add(execution);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicateDealExecution(ex))
        {
            throw new InvalidOperationException("An execution already exists for this deal.", ex);
        }
    }

    private static bool IsDuplicateDealExecution(DbUpdateException exception)
    {
        var sql = exception.InnerException as SqlException
                  ?? exception.InnerException?.InnerException as SqlException;
        if (sql is null)
            return false;

        if (sql.Number is not (2601 or 2627))
            return false;

        return sql.Message.Contains("UX_ServiceExecutions_DealId", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyList<ServiceExecution>> ListVisibleByDealAsync(
        long dealId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ServiceExecutions
            .AsNoTracking()
            .Where(e =>
                e.DealId == dealId
                && !e.Deal.IsDeleted
                && !e.Deal.Request.IsDeleted
                && (e.Deal.Request.Customer.UserId == userId
                    || (e.Deal.Proposal.ProviderId != null && e.Deal.Proposal.Provider!.UserId == userId)
                    || (e.Deal.Proposal.BusinessId != null && e.Deal.Proposal.Business!.OwnerUserId == userId)
                    || e.Assignments.Any(a => a.Provider.UserId == userId)))
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<ServiceExecution?> GetVisibleByIdAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return _context.ServiceExecutions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.Id == executionId
                     && !e.Deal.IsDeleted
                     && !e.Deal.Request.IsDeleted
                     && (e.Deal.Request.Customer.UserId == userId
                         || (e.Deal.Proposal.ProviderId != null && e.Deal.Proposal.Provider!.UserId == userId)
                         || (e.Deal.Proposal.BusinessId != null && e.Deal.Proposal.Business!.OwnerUserId == userId)
                         || e.Assignments.Any(a => a.Provider.UserId == userId)),
                cancellationToken);
    }

    public Task<ServiceExecution?> GetTrackedForPartyAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return QueryTracked()
            .FirstOrDefaultAsync(
                e => e.Id == executionId
                     && !e.Deal.IsDeleted
                     && !e.Deal.Request.IsDeleted
                     && ((e.Deal.Proposal.ProviderId != null && e.Deal.Proposal.Provider!.UserId == userId)
                         || (e.Deal.Proposal.BusinessId != null && e.Deal.Proposal.Business!.OwnerUserId == userId)),
                cancellationToken);
    }

    public Task<ServiceExecution?> GetTrackedForStartCompleteAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return QueryTracked()
            .FirstOrDefaultAsync(
                e => e.Id == executionId
                     && !e.Deal.IsDeleted
                     && !e.Deal.Request.IsDeleted
                     && ((e.Deal.Proposal.ProviderId != null && e.Deal.Proposal.Provider!.UserId == userId)
                         || (e.Deal.Proposal.BusinessId != null && e.Deal.Proposal.Business!.OwnerUserId == userId)
                         || e.Assignments.Any(a =>
                             a.IsPrimary
                             && a.Status == "Assigned"
                             && a.Provider.UserId == userId)),
                cancellationToken);
    }

    public Task<ServiceExecution?> GetTrackedWithAssignmentsForPartyAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return GetTrackedForPartyAsync(executionId, userId, cancellationToken);
    }

    private IQueryable<ServiceExecution> QueryTracked()
    {
        return _context.ServiceExecutions
            .Include(e => e.Assignments)
            .Include(e => e.Deal)
                .ThenInclude(d => d.Proposal)
                    .ThenInclude(p => p.Provider)
            .Include(e => e.Deal)
                .ThenInclude(d => d.Proposal)
                    .ThenInclude(p => p.Business)
            .Include(e => e.Deal)
                .ThenInclude(d => d.Request)
                    .ThenInclude(r => r.Customer);
    }
}

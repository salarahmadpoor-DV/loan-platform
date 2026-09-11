using Matchi.Application.Common;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
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
            throw new ConflictException("A deal already exists for this proposal.", ex);
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

    public async Task<IReadOnlyList<Deal>> ListVisibleToProviderAsync(
        long providerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Deals
            .AsNoTracking()
            .Include(deal => deal.Proposal)
            .Where(deal =>
                !deal.IsDeleted
                && !deal.Request.IsDeleted
                && (deal.Proposal.ProviderId == providerId
                    || deal.ServiceExecutions.Any(execution =>
                        execution.Assignments.Any(assignment =>
                            assignment.ProviderId == providerId
                            && assignment.Status == "Assigned"))))
            .OrderByDescending(deal => deal.AcceptedAt)
            .ThenByDescending(deal => deal.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> HasActiveDealForRequestAsync(
        long requestId,
        CancellationToken cancellationToken = default)
    {
        return _context.Deals.AnyAsync(
            d => d.RequestId == requestId
                 && !d.IsDeleted
                 && d.Status == "Active",
            cancellationToken);
    }

    private static bool IsDuplicateProposalDeal(DbUpdateException exception)
    {
        return SqlServerUpdateConflicts.IsUniqueIndex(exception, "UX_Deals_ProposalId");
    }
}

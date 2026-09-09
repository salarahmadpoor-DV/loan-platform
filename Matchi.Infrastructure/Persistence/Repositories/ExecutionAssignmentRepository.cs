using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class ExecutionAssignmentRepository : IExecutionAssignmentRepository
{
    private readonly MatchiDbContext _context;

    public ExecutionAssignmentRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public Task<bool> HasPrimaryAsync(long executionId, CancellationToken cancellationToken = default)
    {
        return _context.ExecutionAssignments.AnyAsync(
            a => a.ServiceExecutionId == executionId
                 && a.IsPrimary
                 && a.Status == "Assigned",
            cancellationToken);
    }

    public Task<bool> HasActiveMembershipAsync(
        long businessId,
        long providerId,
        CancellationToken cancellationToken = default)
    {
        return _context.BusinessProviders.AnyAsync(
            m => m.BusinessId == businessId
                 && m.ProviderId == providerId
                 && !m.IsDeleted
                 && m.Status == "Active",
            cancellationToken);
    }

    public Task<bool> ProviderExistsAsync(long providerId, CancellationToken cancellationToken = default)
    {
        return _context.Providers.AnyAsync(p => p.Id == providerId && !p.IsDeleted, cancellationToken);
    }

    public void Add(ExecutionAssignment assignment)
    {
        _context.ExecutionAssignments.Add(assignment);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExecutionAssignment>> ListVisibleAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ExecutionAssignments
            .AsNoTracking()
            .Where(a =>
                a.ServiceExecutionId == executionId
                && !a.ServiceExecution.Deal.IsDeleted
                && !a.ServiceExecution.Deal.Request.IsDeleted
                && (a.ServiceExecution.Deal.Request.Customer.UserId == userId
                    || (a.ServiceExecution.Deal.Proposal.ProviderId != null
                        && a.ServiceExecution.Deal.Proposal.Provider!.UserId == userId)
                    || (a.ServiceExecution.Deal.Proposal.BusinessId != null
                        && a.ServiceExecution.Deal.Proposal.Business!.OwnerUserId == userId)
                    || a.ServiceExecution.Assignments.Any(x => x.Provider.UserId == userId)))
            .OrderBy(a => a.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<ExecutionAssignment?> GetTrackedForBusinessOwnerAsync(
        long executionId,
        long assignmentId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return _context.ExecutionAssignments
            .Include(a => a.ServiceExecution)
                .ThenInclude(e => e.Deal)
                    .ThenInclude(d => d.Proposal)
                        .ThenInclude(p => p.Business)
            .Include(a => a.ServiceExecution)
                .ThenInclude(e => e.Deal)
                    .ThenInclude(d => d.Request)
            .FirstOrDefaultAsync(
                a => a.Id == assignmentId
                     && a.ServiceExecutionId == executionId
                     && !a.ServiceExecution.Deal.IsDeleted
                     && !a.ServiceExecution.Deal.Request.IsDeleted
                     && a.ServiceExecution.Deal.Proposal.BusinessId != null
                     && a.ServiceExecution.Deal.Proposal.Business!.OwnerUserId == userId,
                cancellationToken);
    }
}

using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Repositories;

public sealed class ProposalRepository : IProposalRepository
{
    private readonly MatchiDbContext _context;

    public ProposalRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Proposal proposal, CancellationToken cancellationToken = default)
    {
        await _context.Proposals.AddAsync(proposal, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> IsRequestOwnedByUserAsync(
        long requestId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Requests.AnyAsync(
            r => r.Id == requestId && !r.IsDeleted && r.Customer.UserId == userId,
            cancellationToken);
    }

    public Task<Proposal?> GetTrackedOwnedByRequestOwnerAsync(
        long proposalId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Proposals
            .Include(p => p.Request)
                .ThenInclude(r => r.Customer)
            .FirstOrDefaultAsync(
                p => p.Id == proposalId
                     && !p.IsDeleted
                     && !p.Request.IsDeleted
                     && p.Request.Customer.UserId == userId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Proposal>> ListOwnedRequestProposalsAsync(
        long requestId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .AsNoTracking()
            .Where(p =>
                p.RequestId == requestId
                && !p.IsDeleted
                && !p.Request.IsDeleted
                && p.Request.Customer.UserId == userId)
            .OrderByDescending(p => p.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public Task<Proposal?> GetOwnedDetailAsync(
        long proposalId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Proposals
            .AsNoTracking()
            .Include(p => p.Items)
            .FirstOrDefaultAsync(
                p => p.Id == proposalId
                     && !p.IsDeleted
                     && !p.Request.IsDeleted
                     && p.Request.Customer.UserId == userId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Proposal>> ListByProviderIdAsync(
        long providerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .AsNoTracking()
            .Include(proposal => proposal.Deal)
            .Where(proposal =>
                proposal.ProviderId == providerId
                && !proposal.IsDeleted
                && !proposal.Request.IsDeleted)
            .OrderByDescending(proposal => proposal.CreateDate)
            .ThenByDescending(proposal => proposal.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<long>> GetActiveServiceIdsAsync(
        IReadOnlyCollection<long> serviceIds,
        CancellationToken cancellationToken = default)
    {
        if (serviceIds.Count == 0)
            return [];

        return await _context.Services
            .AsNoTracking()
            .Where(s => serviceIds.Contains(s.Id) && !s.IsDeleted && s.IsActive)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<long>> GetActiveProductIdsAsync(
        IReadOnlyCollection<long> productIds,
        CancellationToken cancellationToken = default)
    {
        if (productIds.Count == 0)
            return [];

        return await _context.Products
            .AsNoTracking()
            .Where(p => productIds.Contains(p.Id) && !p.IsDeleted && p.IsActive)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<long>> GetOfferedServiceIdsAsync(
        bool asProvider,
        long partyId,
        IReadOnlyCollection<long> serviceIds,
        CancellationToken cancellationToken = default)
    {
        if (serviceIds.Count == 0)
            return [];

        if (asProvider)
        {
            return await _context.ProviderServices
                .AsNoTracking()
                .Where(x =>
                    x.ProviderId == partyId
                    && serviceIds.Contains(x.ServiceId)
                    && !x.IsDeleted
                    && x.IsActive)
                .Select(x => x.ServiceId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        return await _context.BusinessServices
            .AsNoTracking()
            .Where(x =>
                x.BusinessId == partyId
                && serviceIds.Contains(x.ServiceId)
                && !x.IsDeleted
                && x.IsActive)
            .Select(x => x.ServiceId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<long>> GetOfferedProductIdsAsync(
        bool asProvider,
        long partyId,
        IReadOnlyCollection<long> productIds,
        CancellationToken cancellationToken = default)
    {
        if (productIds.Count == 0)
            return [];

        if (asProvider)
        {
            return await _context.ProviderProducts
                .AsNoTracking()
                .Where(x =>
                    x.ProviderId == partyId
                    && productIds.Contains(x.ProductId)
                    && !x.IsDeleted
                    && x.IsAvailable)
                .Select(x => x.ProductId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        return await _context.BusinessProducts
            .AsNoTracking()
            .Where(x =>
                x.BusinessId == partyId
                && productIds.Contains(x.ProductId)
                && !x.IsDeleted
                && x.IsAvailable)
            .Select(x => x.ProductId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}

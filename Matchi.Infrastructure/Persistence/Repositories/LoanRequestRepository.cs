using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;

namespace Matchi.Infrastructure.Persistence.Repositories;

public class LoanRequestRepository : ILoanRequestRepository
{
    private readonly MatchiDbContext _context;

    public LoanRequestRepository(MatchiDbContext context)
    {
        _context = context;
    }

    public async Task<long> CreateAsync(
        long bankId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        var request = new LoanRequest(
            bankId,
            userId);

        await _context.LoanRequests.AddAsync(
            request,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return request.Id;
    }
}
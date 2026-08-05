using Loan.Domain.Entities;
using Loan.Domain.Interfaces;

namespace Loan.Infrastructure.Persistence.Repositories;

public class LoanRequestRepository : ILoanRequestRepository
{
    private readonly LoanDbContext _context;

    public LoanRequestRepository(LoanDbContext context)
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
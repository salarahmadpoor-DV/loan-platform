using Loan.Domain.Entities;
using Loan.Domain.Interfaces;
using Loan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Loan.Infrastructure.Persistence.Repositories;

public class BankRepository : IBankRepository
{
    private readonly LoanDbContext _context;

    public BankRepository(LoanDbContext context)
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
}
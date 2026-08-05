using Loan.Domain.Entities;

namespace Loan.Domain.Interfaces;

public interface IBankRepository
{
    Task<List<Bank>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
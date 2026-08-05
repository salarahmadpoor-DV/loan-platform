using Loan.Domain.Entities;

namespace Loan.Domain.Interfaces;

public interface ILoanRequestRepository
{
    Task<long> CreateAsync(
        long bankId,
        long userId,
        CancellationToken cancellationToken = default);
}
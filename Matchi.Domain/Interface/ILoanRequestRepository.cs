using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface ILoanRequestRepository
{
    Task<long> CreateAsync(
        long bankId,
        long userId,
        CancellationToken cancellationToken = default);
}
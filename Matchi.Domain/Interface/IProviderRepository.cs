 
using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IProviderRepository
{
    Task<Provider?> GetByIdAsync(
        long providerId,
        CancellationToken cancellationToken = default);

    Task<Provider?> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Provider provider,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Provider>> GetProvidersByServiceIdAsync(
        long serviceId,
        CancellationToken cancellationToken = default);
}
 

using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IIntroductionRepository
{
    Task<long> AddAsync(Introduction introduction, CancellationToken cancellationToken = default);
    Task<IEnumerable<Introduction>> GetByRequestIdAsync(long requestId, CancellationToken cancellationToken = default);
    Task<Introduction?> GetByIdAsync(long introductionId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Introduction introduction, CancellationToken cancellationToken = default);
}

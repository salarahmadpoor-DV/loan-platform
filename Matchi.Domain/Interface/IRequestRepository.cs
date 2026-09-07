using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IRequestRepository
{
    Task<long> CreateServiceRequestAsync(
        long userId,
        long serviceId,
        string title,
        string? description,
        decimal? lat,
        decimal? lng,
        IEnumerable<(long AttributeId, string? Value)> answers,
        CancellationToken cancellationToken = default);

    Task<Request?> GetByIdAsync(long requestId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Request>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
}

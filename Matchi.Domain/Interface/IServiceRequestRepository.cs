using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IServiceRequestRepository
{
    Task<long> CreateRequestAsync(ServiceRequest request, IEnumerable<RequestAnswer> answers, CancellationToken cancellationToken = default);
    Task<ServiceRequest?> GetByIdAsync(long requestId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServiceRequest>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
}
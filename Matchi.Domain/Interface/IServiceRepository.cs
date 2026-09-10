using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IServiceRepository
{
    Task<IEnumerable<ServiceCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Service>> GetServicesAsync(
        long? categoryId,
        string? query,
        int skip,
        int take,
        CancellationToken cancellationToken = default);
    Task<Service?> GetServiceByIdAsync(long serviceId, CancellationToken cancellationToken = default);
}
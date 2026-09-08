using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IRequestRepository
{
    Task<Customer> GetOrCreateCustomerAsync(long userId, CancellationToken cancellationToken = default);

    Task<bool> ServiceExistsAsync(long serviceId, CancellationToken cancellationToken = default);

    Task<ServiceAttribute?> GetServiceAttributeAsync(
        long serviceId,
        long serviceAttributeId,
        CancellationToken cancellationToken = default);

    Task<Product?> GetProductAsync(long productId, CancellationToken cancellationToken = default);

    Task<bool> ProductCategoryExistsAsync(long productCategoryId, CancellationToken cancellationToken = default);

    Task<ProductAttribute?> GetProductAttributeAsync(
        long productCategoryId,
        long productAttributeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Request request, CancellationToken cancellationToken = default);

    Task UpdateAsync(Request request, CancellationToken cancellationToken = default);

    void RemoveLocationsAndSchedules(Request request);

    Task<Request?> GetByIdAsync(
        long requestId,
        CancellationToken cancellationToken = default);

    Task<Request?> GetOwnedByIdAsync(
        long requestId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Request>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);
}

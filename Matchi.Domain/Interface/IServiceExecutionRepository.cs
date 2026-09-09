using Matchi.Domain.Entities;

namespace Matchi.Domain.Interfaces;

public interface IServiceExecutionRepository
{
    Task<Deal?> GetActiveDealGraphAsync(long dealId, CancellationToken cancellationToken = default);

    Task<bool> ExistsForDealAsync(long dealId, CancellationToken cancellationToken = default);

    Task<bool> IsDealVisibleAsync(long dealId, long userId, CancellationToken cancellationToken = default);

    void Add(ServiceExecution execution);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ServiceExecution>> ListVisibleByDealAsync(
        long dealId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<ServiceExecution?> GetVisibleByIdAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<ServiceExecution?> GetTrackedForPartyAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<ServiceExecution?> GetTrackedForStartCompleteAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<ServiceExecution?> GetTrackedWithAssignmentsForPartyAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default);
}

public interface IExecutionAssignmentRepository
{
    Task<bool> HasPrimaryAsync(long executionId, CancellationToken cancellationToken = default);

    Task<bool> HasActiveMembershipAsync(
        long businessId,
        long providerId,
        CancellationToken cancellationToken = default);

    Task<bool> ProviderExistsAsync(long providerId, CancellationToken cancellationToken = default);

    void Add(ExecutionAssignment assignment);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExecutionAssignment>> ListVisibleAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<ExecutionAssignment?> GetTrackedForBusinessOwnerAsync(
        long executionId,
        long assignmentId,
        long userId,
        CancellationToken cancellationToken = default);
}

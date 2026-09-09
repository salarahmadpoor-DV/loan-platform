using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Tests;

internal sealed class FakeCurrentUser : ICurrentUserService
{
    public FakeCurrentUser(long userId)
    {
        UserId = userId;
        IsAuthenticated = true;
    }

    public bool IsAuthenticated { get; }

    public long? UserId { get; }

    public string? Mobile => null;
}

internal sealed class FakeServiceExecutionRepository : IServiceExecutionRepository
{
    public Deal? DealGraph { get; set; }
    public ServiceExecution? Tracked { get; set; }
    public bool Exists { get; set; }
    public bool DealVisible { get; set; } = true;
    public List<ServiceExecution> Added { get; } = [];
    public int SaveCount { get; private set; }

    public Task<Deal?> GetActiveDealGraphAsync(long dealId, CancellationToken cancellationToken = default) =>
        Task.FromResult(DealGraph is not null && DealGraph.Id == dealId ? DealGraph : null);

    public Task<bool> ExistsForDealAsync(long dealId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Exists);

    public Task<bool> IsDealVisibleAsync(long dealId, long userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(DealVisible);

    public void Add(ServiceExecution execution)
    {
        execution.WithId(Added.Count + 1);
        Added.Add(execution);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ServiceExecution>> ListVisibleByDealAsync(
        long dealId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<ServiceExecution>>([]);

    public Task<ServiceExecution?> GetVisibleByIdAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Tracked is not null && Tracked.Id == executionId ? Tracked : null);

    public Task<ServiceExecution?> GetTrackedForPartyAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Tracked);

    public Task<ServiceExecution?> GetTrackedForStartCompleteAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Tracked);

    public Task<ServiceExecution?> GetTrackedWithAssignmentsForPartyAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Tracked);
}

internal sealed class FakeExecutionAssignmentRepository : IExecutionAssignmentRepository
{
    public bool HasPrimary { get; set; }
    public bool HasMembership { get; set; } = true;
    public bool ProviderExists { get; set; } = true;
    public ExecutionAssignment? Tracked { get; set; }
    public List<ExecutionAssignment> Added { get; } = [];
    public int SaveCount { get; private set; }

    public Task<bool> HasPrimaryAsync(long executionId, CancellationToken cancellationToken = default) =>
        Task.FromResult(HasPrimary);

    public Task<bool> HasActiveMembershipAsync(
        long businessId,
        long providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(HasMembership);

    public Task<bool> ProviderExistsAsync(long providerId, CancellationToken cancellationToken = default) =>
        Task.FromResult(ProviderExists);

    public void Add(ExecutionAssignment assignment)
    {
        assignment.WithId(Added.Count + 1);
        Added.Add(assignment);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ExecutionAssignment>> ListVisibleAsync(
        long executionId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<ExecutionAssignment>>([]);

    public Task<ExecutionAssignment?> GetTrackedForBusinessOwnerAsync(
        long executionId,
        long assignmentId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Tracked);
}

internal sealed class FakeReviewRepository : IReviewRepository
{
    public Deal? Deal { get; set; }
    public bool Exists { get; set; }
    public List<Review> Added { get; } = [];
    public List<Review> ProviderReviews { get; } = [];
    public List<Review> BusinessReviews { get; } = [];

    public Task<Deal?> GetCustomerDealGraphAsync(
        long dealId,
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(
            Deal is not null
            && Deal.Id == dealId
            && Deal.Request.Customer.UserId == userId
                ? Deal
                : null);

    public Task<bool> ExistsActiveTargetAsync(
        long dealId,
        long customerId,
        long? businessId,
        long? providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Exists);

    public Task<IReadOnlyList<Review>> ListByProviderAsync(
        long providerId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Review>>(
            ProviderReviews.Where(r => r.ProviderId == providerId && !r.IsDeleted).ToList());

    public Task<IReadOnlyList<Review>> ListByBusinessAsync(
        long businessId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Review>>(
            BusinessReviews.Where(r => r.BusinessId == businessId && !r.IsDeleted).ToList());

    public void Add(Review review)
    {
        review.WithId(Added.Count + 1);
        Added.Add(review);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

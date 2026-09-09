using Matchi.Domain.Entities;

namespace Matchi.Application.Tests;

public sealed class ServiceExecutionDomainTests
{
    [Fact]
    public void Create_ForIndependentProvider_AllowsNullBusinessId()
    {
        var execution = ServiceExecution.Create(1, null);
        Assert.Equal("Pending", execution.Status);
        Assert.Null(execution.BusinessId);
    }

    [Fact]
    public void UpdateSchedule_WhenPending_AllowsNullableTimes()
    {
        var execution = ServiceExecution.Create(1, null);
        execution.UpdateSchedule(new DateOnly(2026, 9, 10), null, null);
        Assert.Equal(new DateOnly(2026, 9, 10), execution.ScheduledDate);
        Assert.Null(execution.ScheduledTimeFrom);
    }

    [Fact]
    public void UpdateSchedule_RejectsFromNotBeforeTo()
    {
        var execution = ServiceExecution.Create(1, null);
        Assert.Throws<InvalidOperationException>(() =>
            execution.UpdateSchedule(null, TimeSpan.FromHours(11), TimeSpan.FromHours(10)));
    }

    [Fact]
    public void UpdateSchedule_RejectsWhenNotPending()
    {
        var execution = ServiceExecution.Create(1, null);
        execution.Start();
        Assert.Throws<InvalidOperationException>(() =>
            execution.UpdateSchedule(DateOnly.FromDateTime(DateTime.UtcNow), null, null));
    }

    [Fact]
    public void Start_SetsUtcStartedAt_AndRejectsRepeat()
    {
        var execution = ServiceExecution.Create(1, 7);
        execution.Start();
        Assert.Equal("InProgress", execution.Status);
        Assert.NotNull(execution.StartedAt);
        Assert.True(Math.Abs((DateTime.UtcNow - execution.StartedAt!.Value).TotalSeconds) < 5);
        Assert.Throws<InvalidOperationException>(() => execution.Start());
    }

    [Fact]
    public void Complete_RequiresInProgress_SetsUtcCompletedAt_AndRejectsRepeat()
    {
        var execution = ServiceExecution.Create(1, null);
        Assert.Throws<InvalidOperationException>(() => execution.Complete());
        execution.Start();
        execution.Complete();
        Assert.Equal("Completed", execution.Status);
        Assert.NotNull(execution.CompletedAt);
        Assert.True(Math.Abs((DateTime.UtcNow - execution.CompletedAt!.Value).TotalSeconds) < 5);
        Assert.Throws<InvalidOperationException>(() => execution.Complete());
    }

    [Fact]
    public void Cancel_RejectsCompletedAndCancelled()
    {
        var pending = ServiceExecution.Create(1, null);
        pending.Cancel();
        Assert.Equal("Cancelled", pending.Status);
        Assert.Throws<InvalidOperationException>(() => pending.Cancel());

        var completed = ServiceExecution.Create(2, null);
        completed.Start();
        completed.Complete();
        Assert.Throws<InvalidOperationException>(() => completed.Cancel());
    }
}

public sealed class ExecutionAssignmentDomainTests
{
    [Fact]
    public void Create_SetsAssignedStatusAndUtcAssignedAt()
    {
        var assignment = ExecutionAssignment.Create(1, 2, "Technician", true);
        Assert.Equal("Assigned", assignment.Status);
        Assert.True(assignment.IsPrimary);
        Assert.True(Math.Abs((DateTime.UtcNow - assignment.AssignedAt).TotalSeconds) < 5);
    }

    [Fact]
    public void Cancel_RejectsRepeat()
    {
        var assignment = ExecutionAssignment.Create(1, 2, "Technician", false);
        assignment.Cancel();
        Assert.Equal("Cancelled", assignment.Status);
        Assert.Throws<InvalidOperationException>(() => assignment.Cancel());
    }
}

public sealed class ReviewDomainTests
{
    [Fact]
    public void Create_RequiresXorTargetAndRatingRange()
    {
        var review = Review.Create(1, 2, 5, "good", null, 9);
        Assert.Equal((byte)5, review.Rating);
        Assert.Equal(9, review.ProviderId);
        Assert.Null(review.BusinessId);

        Assert.Throws<InvalidOperationException>(() => Review.Create(1, 2, 0, null, 3, null));
        Assert.Throws<InvalidOperationException>(() => Review.Create(1, 2, 6, null, 3, null));
        Assert.Throws<InvalidOperationException>(() => Review.Create(1, 2, 4, null, 3, 9));
        Assert.Throws<InvalidOperationException>(() => Review.Create(1, 2, 4, null, null, null));
    }
}

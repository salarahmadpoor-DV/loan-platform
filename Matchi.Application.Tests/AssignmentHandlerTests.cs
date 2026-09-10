using FluentValidation;
using Matchi.Application.Common;
using Matchi.Application.Features.Executions.Commands.CreateExecutionAssignment;
using Matchi.Application.Features.Executions.Commands.RemoveExecutionAssignment;
using Matchi.Domain.Entities;

namespace Matchi.Application.Tests;

public sealed class ExecutionAssignmentCommandTests
{
    private static ServiceExecution BusinessExecution()
    {
        var deal = MarketplaceGraph.BusinessDeal();
        var execution = ServiceExecution.Create(deal.Id, deal.Proposal.BusinessId).WithId(3);
        execution.Set(nameof(ServiceExecution.Deal), deal);
        return execution;
    }

    [Fact]
    public async Task BusinessOwner_CanAssignActiveMember_AndFirstBecomesPrimary()
    {
        var executions = new FakeServiceExecutionRepository { Tracked = BusinessExecution() };
        var assignments = new FakeExecutionAssignmentRepository { HasPrimary = false, HasMembership = true };
        var handler = new CreateExecutionAssignmentCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.BusinessOwnerUserId),
            executions,
            assignments);

        var id = await handler.Handle(
            new CreateExecutionAssignmentCommand(3, 44, "Technician", false),
            CancellationToken.None);

        Assert.Equal(1, id);
        Assert.True(assignments.Added[0].IsPrimary);
    }

    [Fact]
    public async Task NonOwner_CannotAssign()
    {
        var handler = new CreateExecutionAssignmentCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.OtherUserId),
            new FakeServiceExecutionRepository(),
            new FakeExecutionAssignmentRepository());

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new CreateExecutionAssignmentCommand(3, 44, "Technician"), CancellationToken.None));
    }

    [Fact]
    public async Task ProviderParty_CannotReceiveAssignments()
    {
        var deal = MarketplaceGraph.ProviderDeal();
        var execution = ServiceExecution.Create(deal.Id, null).WithId(3);
        execution.Set(nameof(ServiceExecution.Deal), deal);
        var handler = new CreateExecutionAssignmentCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
            new FakeServiceExecutionRepository { Tracked = execution },
            new FakeExecutionAssignmentRepository());

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateExecutionAssignmentCommand(3, 44, "Technician"), CancellationToken.None));
    }

    [Fact]
    public async Task InactiveOrNonMemberProvider_IsRejected()
    {
        var handler = new CreateExecutionAssignmentCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.BusinessOwnerUserId),
            new FakeServiceExecutionRepository { Tracked = BusinessExecution() },
            new FakeExecutionAssignmentRepository { HasMembership = false });

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateExecutionAssignmentCommand(3, 44, "Technician"), CancellationToken.None));
    }

    [Fact]
    public async Task SecondPrimary_IsRejected()
    {
        var handler = new CreateExecutionAssignmentCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.BusinessOwnerUserId),
            new FakeServiceExecutionRepository { Tracked = BusinessExecution() },
            new FakeExecutionAssignmentRepository { HasPrimary = true });

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateExecutionAssignmentCommand(3, 44, "Technician", true), CancellationToken.None));
    }

    [Fact]
    public async Task DuplicateAssignedProvider_IsConflict()
    {
        var handler = new CreateExecutionAssignmentCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.BusinessOwnerUserId),
            new FakeServiceExecutionRepository { Tracked = BusinessExecution() },
            new FakeExecutionAssignmentRepository { HasAssignedProvider = true });

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new CreateExecutionAssignmentCommand(3, 44, "Technician"), CancellationToken.None));
    }

    [Fact]
    public async Task Reassignment_AfterCancel_IsAllowedWhenNoAssignedRow()
    {
        var assignment = ExecutionAssignment.Create(3, 44, "Technician", true);
        assignment.Cancel();
        Assert.Equal("Cancelled", assignment.Status);

        var assignments = new FakeExecutionAssignmentRepository { HasAssignedProvider = false };
        var handler = new CreateExecutionAssignmentCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.BusinessOwnerUserId),
            new FakeServiceExecutionRepository { Tracked = BusinessExecution() },
            assignments);

        var id = await handler.Handle(
            new CreateExecutionAssignmentCommand(3, 44, "Technician"),
            CancellationToken.None);

        Assert.Equal(1, id);
        Assert.Equal("Assigned", assignments.Added[0].Status);
    }

    [Fact]
    public async Task DuplicatePrimaryRace_IsConflict()
    {
        var handler = new CreateExecutionAssignmentCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.BusinessOwnerUserId),
            new FakeServiceExecutionRepository { Tracked = BusinessExecution() },
            new FakeExecutionAssignmentRepository
            {
                HasPrimary = false,
                SaveException = new ConflictException("A primary assignment already exists.")
            });

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new CreateExecutionAssignmentCommand(3, 44, "Technician", true), CancellationToken.None));
    }

    [Fact]
    public async Task Remove_RequiresBusinessOwnerAndPendingExecution()
    {
        var execution = BusinessExecution();
        var assignment = ExecutionAssignment.Create(execution.Id, 44, "Tech", true).WithId(9);
        assignment.Set(nameof(ExecutionAssignment.ServiceExecution), execution);

        var handler = new RemoveExecutionAssignmentCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.BusinessOwnerUserId),
            new FakeExecutionAssignmentRepository { Tracked = assignment });

        await handler.Handle(new RemoveExecutionAssignmentCommand(3, 9), CancellationToken.None);
        Assert.Equal("Cancelled", assignment.Status);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new RemoveExecutionAssignmentCommandHandler(
                new FakeCurrentUser(MarketplaceGraph.OtherUserId),
                new FakeExecutionAssignmentRepository()).Handle(
                new RemoveExecutionAssignmentCommand(3, 9),
                CancellationToken.None));
    }

    [Fact]
    public async Task Remove_RejectedWhenExecutionNotPending()
    {
        var execution = BusinessExecution();
        execution.Start();
        var assignment = ExecutionAssignment.Create(execution.Id, 44, "Tech", true).WithId(9);
        assignment.Set(nameof(ExecutionAssignment.ServiceExecution), execution);
        var handler = new RemoveExecutionAssignmentCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.BusinessOwnerUserId),
            new FakeExecutionAssignmentRepository { Tracked = assignment });

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new RemoveExecutionAssignmentCommand(3, 9), CancellationToken.None));
    }
}

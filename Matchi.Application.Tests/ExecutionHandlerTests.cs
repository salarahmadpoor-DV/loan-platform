using FluentValidation;
using Matchi.Application.Features.Executions.Commands.CancelServiceExecution;
using Matchi.Application.Features.Executions.Commands.CompleteServiceExecution;
using Matchi.Application.Features.Executions.Commands.CreateServiceExecution;
using Matchi.Application.Features.Executions.Commands.StartServiceExecution;
using Matchi.Application.Features.Executions.Commands.UpdateServiceExecutionSchedule;
using Matchi.Domain.Entities;

namespace Matchi.Application.Tests;

public sealed class CreateServiceExecutionCommandHandlerTests
{
    [Fact]
    public async Task Create_ForProviderProposal_Succeeds()
    {
        var executions = new FakeServiceExecutionRepository { DealGraph = MarketplaceGraph.ProviderDeal() };
        var handler = new CreateServiceExecutionCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
            executions);

        var id = await handler.Handle(new CreateServiceExecutionCommand(1), CancellationToken.None);

        Assert.Equal(1, id);
        Assert.Null(executions.Added[0].BusinessId);
        Assert.Equal(1, executions.SaveCount);
    }

    [Fact]
    public async Task Create_ForBusinessProposal_CopiesBusinessId()
    {
        var executions = new FakeServiceExecutionRepository { DealGraph = MarketplaceGraph.BusinessDeal() };
        var handler = new CreateServiceExecutionCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.BusinessOwnerUserId),
            executions);

        await handler.Handle(new CreateServiceExecutionCommand(2), CancellationToken.None);

        Assert.Equal(7, executions.Added[0].BusinessId);
    }

    [Fact]
    public async Task Create_RejectsUnauthorizedUser()
    {
        var executions = new FakeServiceExecutionRepository { DealGraph = MarketplaceGraph.ProviderDeal() };
        var handler = new CreateServiceExecutionCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.OtherUserId),
            executions);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new CreateServiceExecutionCommand(1), CancellationToken.None));
    }

    [Fact]
    public async Task Create_RejectsInactiveDeal()
    {
        var executions = new FakeServiceExecutionRepository
        {
            DealGraph = MarketplaceGraph.ProviderDeal(dealStatus: "Cancelled")
        };
        var handler = new CreateServiceExecutionCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
            executions);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateServiceExecutionCommand(1), CancellationToken.None));
    }

    [Fact]
    public async Task Create_RejectsProductOnlyRequest()
    {
        var executions = new FakeServiceExecutionRepository
        {
            DealGraph = MarketplaceGraph.ProviderDeal(requestType: "Product")
        };
        var handler = new CreateServiceExecutionCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
            executions);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateServiceExecutionCommand(1), CancellationToken.None));
    }

    [Fact]
    public async Task Create_RejectsDuplicateExecution()
    {
        var executions = new FakeServiceExecutionRepository
        {
            DealGraph = MarketplaceGraph.ProviderDeal(),
            Exists = true
        };
        var handler = new CreateServiceExecutionCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
            executions);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateServiceExecutionCommand(1), CancellationToken.None));
    }
}

public sealed class ServiceExecutionLifecycleHandlerTests
{
    [Fact]
    public async Task Schedule_AllowedOnlyWhilePending()
    {
        var pending = ServiceExecution.Create(1, null).WithId(1);
        pending.Set(nameof(ServiceExecution.Deal), MarketplaceGraph.ProviderDeal());
        var executions = new FakeServiceExecutionRepository { Tracked = pending };
        var handler = new UpdateServiceExecutionScheduleCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
            executions);

        await handler.Handle(
            new UpdateServiceExecutionScheduleCommand(1, null, TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
            CancellationToken.None);

        pending.Start();
        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(
                new UpdateServiceExecutionScheduleCommand(1, null, TimeSpan.FromHours(9), TimeSpan.FromHours(11)),
                CancellationToken.None));
    }

    [Fact]
    public async Task Start_ValidThenRejectsRepeat()
    {
        var execution = ServiceExecution.Create(1, null).WithId(1);
        var executions = new FakeServiceExecutionRepository { Tracked = execution };
        var handler = new StartServiceExecutionCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
            executions);

        var status = await handler.Handle(new StartServiceExecutionCommand(1), CancellationToken.None);
        Assert.Equal("InProgress", status);
        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new StartServiceExecutionCommand(1), CancellationToken.None));
    }

    [Fact]
    public async Task Complete_ValidThenRejectsRepeat()
    {
        var execution = ServiceExecution.Create(1, null).WithId(1);
        execution.Start();
        var executions = new FakeServiceExecutionRepository { Tracked = execution };
        var handler = new CompleteServiceExecutionCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
            executions);

        var status = await handler.Handle(new CompleteServiceExecutionCommand(1), CancellationToken.None);
        Assert.Equal("Completed", status);
        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CompleteServiceExecutionCommand(1), CancellationToken.None));
    }

    [Fact]
    public async Task Cancel_ValidAndCancelsAssignedRows()
    {
        var execution = ServiceExecution.Create(1, 7).WithId(1);
        var assignment = ExecutionAssignment.Create(1, 4, "Tech", true);
        execution.Assignments.Add(assignment);
        var executions = new FakeServiceExecutionRepository { Tracked = execution };
        var handler = new CancelServiceExecutionCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.BusinessOwnerUserId),
            executions);

        var status = await handler.Handle(new CancelServiceExecutionCommand(1), CancellationToken.None);
        Assert.Equal("Cancelled", status);
        Assert.Equal("Cancelled", assignment.Status);
        Assert.Equal(1, executions.SaveCount);
    }

    [Fact]
    public async Task Cancel_RejectsUnauthorizedMissingExecution()
    {
        var handler = new CancelServiceExecutionCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.OtherUserId),
            new FakeServiceExecutionRepository());

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new CancelServiceExecutionCommand(1), CancellationToken.None));
    }
}

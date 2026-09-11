using Matchi.Application.Features.Providers;
using Matchi.Application.Features.Providers.Queries;
using Matchi.Domain.Entities;

namespace Matchi.Application.Tests;

public sealed class ProviderMarketplaceVisibilityTests
{
    [Fact]
    public void Deal_IsVisible_WhenProposalBelongsToProvider()
    {
        var deal = MarketplaceGraph.ProviderDeal();
        Assert.True(ProviderMarketplaceVisibility.CanSeeDeal(deal, MarketplaceGraph.ProviderEntityId));
        Assert.False(ProviderMarketplaceVisibility.CanSeeDeal(deal, MarketplaceGraph.OtherProviderEntityId));
    }

    [Fact]
    public void Deal_IsNotVisible_FromBusinessMembershipAlone()
    {
        var deal = MarketplaceGraph.BusinessDeal();
        Assert.False(ProviderMarketplaceVisibility.CanSeeDeal(deal, MarketplaceGraph.ProviderEntityId));
    }

    [Fact]
    public void Deal_IsVisible_WhenProviderIsAssignedExecutor()
    {
        var deal = MarketplaceGraph.BusinessDeal();
        AttachAssignedExecution(deal, MarketplaceGraph.ProviderEntityId, isPrimary: true);

        Assert.True(ProviderMarketplaceVisibility.CanSeeDeal(deal, MarketplaceGraph.ProviderEntityId));
        Assert.False(ProviderMarketplaceVisibility.CanSeeDeal(deal, MarketplaceGraph.OtherProviderEntityId));
    }

    [Fact]
    public void Deal_IsNotVisible_WhenAssignmentIsCancelled()
    {
        var deal = MarketplaceGraph.BusinessDeal();
        var assignment = AttachAssignedExecution(deal, MarketplaceGraph.ProviderEntityId, isPrimary: false);
        assignment.Cancel();

        Assert.False(ProviderMarketplaceVisibility.CanSeeDeal(deal, MarketplaceGraph.ProviderEntityId));
    }

    [Fact]
    public void Execution_IsVisible_ForIndependentProposalParty_AndAssignedPrimary()
    {
        var providerDeal = MarketplaceGraph.ProviderDeal();
        var ownExecution = ServiceExecution.Create(providerDeal.Id, null).WithId(1);
        ownExecution.Set(nameof(ServiceExecution.Deal), providerDeal);

        Assert.True(ProviderMarketplaceVisibility.CanSeeExecution(ownExecution, MarketplaceGraph.ProviderEntityId));
        Assert.False(ProviderMarketplaceVisibility.CanSeeExecution(ownExecution, MarketplaceGraph.OtherProviderEntityId));

        var businessDeal = MarketplaceGraph.BusinessDeal();
        var assigned = ServiceExecution.Create(businessDeal.Id, 7).WithId(2);
        assigned.Set(nameof(ServiceExecution.Deal), businessDeal);
        assigned.Assignments.Add(ExecutionAssignment.Create(assigned.Id, MarketplaceGraph.ProviderEntityId, "Tech", true));

        Assert.True(ProviderMarketplaceVisibility.CanSeeExecution(assigned, MarketplaceGraph.ProviderEntityId));
    }

    internal static ExecutionAssignment AttachAssignedExecution(Deal deal, long providerId, bool isPrimary)
    {
        var execution = ServiceExecution.Create(deal.Id, deal.Proposal.BusinessId).WithId(3);
        execution.Set(nameof(ServiceExecution.Deal), deal);
        var assignment = ExecutionAssignment.Create(execution.Id, providerId, "Tech", isPrimary);
        execution.Assignments.Add(assignment);
        deal.ServiceExecutions.Add(execution);
        return assignment;
    }
}

public sealed class ProviderMarketplaceQueryTests
{
    [Fact]
    public async Task Customer_WithoutProviderProfile_CannotLoadProviderLists()
    {
        var providers = new FakeProviderRepository();
        var currentUser = new FakeCurrentUser(MarketplaceGraph.CustomerUserId);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new GetProviderRequestInboxQueryHandler(currentUser, providers, new FakeMatchingReadRepository())
                .Handle(new GetProviderRequestInboxQuery(), CancellationToken.None));

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new GetMyProviderProposalsQueryHandler(currentUser, providers, new FakeProposalRepository())
                .Handle(new GetMyProviderProposalsQuery(), CancellationToken.None));

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new GetMyProviderDealsQueryHandler(currentUser, providers, new FakeDealRepository())
                .Handle(new GetMyProviderDealsQuery(), CancellationToken.None));

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new GetMyProviderExecutionsQueryHandler(currentUser, providers, new FakeServiceExecutionRepository())
                .Handle(new GetMyProviderExecutionsQuery(), CancellationToken.None));
    }

    [Fact]
    public async Task Inbox_ReturnsOnlyEligibleRequests_ForCurrentProvider()
    {
        var matching = new FakeMatchingReadRepository();
        matching.Eligible.Add(new Request(1, "Service", "Eligible").WithId(8));
        matching.Eligible.Add(new Request(1, "Product", "Also eligible").WithId(9));

        var items = await InboxHandler(matching).Handle(new GetProviderRequestInboxQuery(), CancellationToken.None);
        Assert.Equal(2, items.Count);
        Assert.Equal(8, items[0].RequestId);
        Assert.Equal("Service", items[0].RequestType);

        matching.EligibleForProviderId = MarketplaceGraph.OtherProviderEntityId;
        var empty = await InboxHandler(matching)
            .Handle(new GetProviderRequestInboxQuery(), CancellationToken.None);
        Assert.Empty(empty);
    }

    [Fact]
    public async Task Proposals_AreScopedToCurrentProvider_AndIncludeAllStatuses()
    {
        var pending = Proposal.ForProvider(1, MarketplaceGraph.ProviderEntityId, 10m).WithId(1);
        var accepted = Proposal.ForProvider(2, MarketplaceGraph.ProviderEntityId, 20m).WithId(2);
        accepted.Accept();
        var deal = Deal.Create(2, accepted.Id, 1, 20m).WithId(50);
        accepted.Set(nameof(Proposal.Deal), deal);
        var rejected = Proposal.ForProvider(3, MarketplaceGraph.ProviderEntityId, 30m).WithId(3);
        rejected.Reject();
        var other = Proposal.ForProvider(4, MarketplaceGraph.OtherProviderEntityId, 99m).WithId(4);

        var repo = new FakeProposalRepository();
        repo.ByProvider.AddRange([pending, accepted, rejected, other]);

        var items = await new GetMyProviderProposalsQueryHandler(
                new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
                MineProviders(),
                repo)
            .Handle(new GetMyProviderProposalsQuery(), CancellationToken.None);

        Assert.Equal(3, items.Count);
        Assert.Contains(items, item => item.Status == "Pending" && item.DealId is null);
        Assert.Contains(items, item => item.Status == "Accepted" && item.DealId == 50);
        Assert.Contains(items, item => item.Status == "Rejected" && item.DealId is null);
        Assert.DoesNotContain(items, item => item.Id == 4);
    }

    [Fact]
    public async Task Deals_HideOtherProviderAndMembershipOnlyBusinessDeals()
    {
        var own = MarketplaceGraph.ProviderDeal();
        var other = MarketplaceGraph.ProviderDeal().WithId(10);
        other.Proposal.WithId(11);
        other.Proposal.Set(nameof(Proposal.ProviderId), MarketplaceGraph.OtherProviderEntityId);

        var membershipOnly = MarketplaceGraph.BusinessDeal();
        var assigned = MarketplaceGraph.BusinessDeal().WithId(20);
        assigned.Proposal.WithId(21);
        ProviderMarketplaceVisibilityTests.AttachAssignedExecution(
            assigned,
            MarketplaceGraph.ProviderEntityId,
            isPrimary: true);

        var repo = new FakeDealRepository();
        repo.VisibleToProvider.AddRange([own, other, membershipOnly, assigned]);

        var items = await new GetMyProviderDealsQueryHandler(
                new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
                MineProviders(),
                repo)
            .Handle(new GetMyProviderDealsQuery(), CancellationToken.None);

        Assert.Equal(2, items.Count);
        Assert.Contains(items, item => item.Id == own.Id);
        Assert.Contains(items, item => item.Id == assigned.Id);
        Assert.DoesNotContain(items, item => item.Id == membershipOnly.Id);
        Assert.DoesNotContain(items, item => item.ProposalId == other.ProposalId && item.Id == other.Id);
    }

    [Fact]
    public async Task Executions_AreLimitedToPartyOrAssignedProvider()
    {
        var ownDeal = MarketplaceGraph.ProviderDeal();
        var ownExecution = ServiceExecution.Create(ownDeal.Id, null).WithId(1);
        ownExecution.Set(nameof(ServiceExecution.Deal), ownDeal);

        var otherDeal = MarketplaceGraph.ProviderDeal();
        otherDeal.Proposal.Set(nameof(Proposal.ProviderId), MarketplaceGraph.OtherProviderEntityId);
        var otherExecution = ServiceExecution.Create(otherDeal.Id, null).WithId(2);
        otherExecution.Set(nameof(ServiceExecution.Deal), otherDeal);

        var businessDeal = MarketplaceGraph.BusinessDeal();
        var assignedExecution = ServiceExecution.Create(businessDeal.Id, 7).WithId(3);
        assignedExecution.Set(nameof(ServiceExecution.Deal), businessDeal);
        assignedExecution.Assignments.Add(
            ExecutionAssignment.Create(assignedExecution.Id, MarketplaceGraph.ProviderEntityId, "Tech", true));

        var repo = new FakeServiceExecutionRepository();
        repo.VisibleToProvider.AddRange([ownExecution, otherExecution, assignedExecution]);

        var items = await new GetMyProviderExecutionsQueryHandler(
                new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
                MineProviders(),
                repo)
            .Handle(new GetMyProviderExecutionsQuery(), CancellationToken.None);

        Assert.Equal(2, items.Count);
        Assert.Contains(items, item => item.Id == 1);
        Assert.Contains(items, item => item.Id == 3 && item.BusinessId == 7);
        Assert.DoesNotContain(items, item => item.Id == 2);
    }

    private static FakeProviderRepository MineProviders() =>
        new() { Mine = MarketplaceGraph.OwnProvider() };

    private static GetProviderRequestInboxQueryHandler InboxHandler(FakeMatchingReadRepository? matching = null) =>
        new(
            new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
            new FakeProviderRepository { Mine = MarketplaceGraph.OwnProvider() },
            matching ?? new FakeMatchingReadRepository());
}

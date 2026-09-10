using FluentValidation;
using Matchi.Application.Common;
using Matchi.Application.Features.Proposals.Commands.AcceptProposal;
using Matchi.Application.Features.Requests.Commands.CancelRequest;
using Matchi.Application.Features.Requests.Commands.DeleteRequest;
using Matchi.Domain.Entities;

namespace Matchi.Application.Tests;

public sealed class RequestLifecycleTests
{
    private static Request OwnedOpenRequest()
    {
        var customer = new Customer(MarketplaceGraph.CustomerUserId).WithId(1);
        var request = new Request(customer.Id, "Service", "Need work").WithId(1);
        request.Set(nameof(Request.Customer), customer);
        return request;
    }

    [Fact]
    public async Task Owner_CancelsOpenRequest_WithoutActiveDeal()
    {
        var request = OwnedOpenRequest();
        var requests = new FakeRequestRepository { Owned = request };
        var deals = new FakeDealRepository { HasActiveDeal = false };
        var handler = new CancelRequestCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            requests,
            deals);

        var result = await handler.Handle(new CancelRequestCommand(1), CancellationToken.None);

        Assert.True(result);
        Assert.Equal("Cancelled", request.Status);
        Assert.False(request.IsDeleted);
        Assert.Equal(1, requests.UpdateCount);
    }

    [Fact]
    public async Task Owner_Cancel_WithActiveDeal_IsConflict_AndLeavesStateUnchanged()
    {
        var request = OwnedOpenRequest();
        var deal = Deal.Create(request.Id, 8, request.CustomerId, 10m);
        var requests = new FakeRequestRepository { Owned = request };
        var handler = new CancelRequestCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            requests,
            new FakeDealRepository { HasActiveDeal = true });

        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new CancelRequestCommand(1), CancellationToken.None));

        Assert.Equal(409, ExceptionHttpMapper.Map(ex, "t", exposeInternalDetails: false).Status);
        Assert.Equal("Open", request.Status);
        Assert.False(request.IsDeleted);
        Assert.Equal(0, requests.UpdateCount);
        Assert.Equal("Active", deal.Status);
        Assert.False(deal.IsDeleted);
    }

    [Fact]
    public async Task Owner_DeletesRequest_WithoutActiveDeal()
    {
        var request = OwnedOpenRequest();
        var requests = new FakeRequestRepository { Owned = request };
        var handler = new DeleteRequestCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            requests,
            new FakeDealRepository { HasActiveDeal = false });

        var result = await handler.Handle(new DeleteRequestCommand(1), CancellationToken.None);

        Assert.True(result);
        Assert.True(request.IsDeleted);
        Assert.Equal("Open", request.Status);
        Assert.Equal(1, requests.UpdateCount);
    }

    [Fact]
    public async Task Owner_Delete_WithActiveDeal_IsConflict_AndLeavesStateUnchanged()
    {
        var request = OwnedOpenRequest();
        var deal = Deal.Create(request.Id, 8, request.CustomerId, 10m);
        var requests = new FakeRequestRepository { Owned = request };
        var handler = new DeleteRequestCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            requests,
            new FakeDealRepository { HasActiveDeal = true });

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new DeleteRequestCommand(1), CancellationToken.None));

        Assert.False(request.IsDeleted);
        Assert.Equal("Open", request.Status);
        Assert.Equal(0, requests.UpdateCount);
        Assert.Equal("Active", deal.Status);
        Assert.False(deal.IsDeleted);
    }

    [Fact]
    public async Task NonOwner_Cancel_IsNotFound_AndDoesNotQueryActiveDeals()
    {
        var deals = new FakeDealRepository { HasActiveDeal = true, ThrowIfActiveDealQueried = true };
        var handler = new CancelRequestCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.OtherUserId),
            new FakeRequestRepository(),
            deals);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new CancelRequestCommand(1), CancellationToken.None));
        Assert.Equal(0, deals.ActiveDealQueryCount);
    }

    [Fact]
    public async Task NonOwner_Delete_IsNotFound_AndDoesNotQueryActiveDeals()
    {
        var deals = new FakeDealRepository { HasActiveDeal = true, ThrowIfActiveDealQueried = true };
        var handler = new DeleteRequestCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.OtherUserId),
            new FakeRequestRepository(),
            deals);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteRequestCommand(1), CancellationToken.None));
        Assert.Equal(0, deals.ActiveDealQueryCount);
    }

    [Fact]
    public async Task Cancel_WhenNotOpen_RemainsValidation()
    {
        var request = OwnedOpenRequest();
        request.Cancel();
        var handler = new CancelRequestCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            new FakeRequestRepository { Owned = request },
            new FakeDealRepository { HasActiveDeal = true, ThrowIfActiveDealQueried = true });

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CancelRequestCommand(1), CancellationToken.None));
    }
}

public sealed class AcceptProposalLifecycleTests
{
    private static Proposal PendingProviderProposal(long proposalId, Request request)
    {
        var provider = new Provider(MarketplaceGraph.ProviderUserId, "Prov", "09120000000").WithId(5);
        var proposal = Proposal.ForProvider(request.Id, provider.Id, 100m).WithId(proposalId);
        proposal.Set(nameof(Proposal.Provider), provider);
        proposal.Set(nameof(Proposal.Request), request);
        return proposal;
    }

    [Fact]
    public async Task Accept_CreatesDeal_LeavesRequestOpen_AndAllowsSecondDeal()
    {
        var request = new Request(1, "Service", "Need work").WithId(1);
        request.Set(nameof(Request.Customer), new Customer(MarketplaceGraph.CustomerUserId).WithId(1));
        var first = PendingProviderProposal(11, request);
        var second = PendingProviderProposal(12, request);
        var proposals = new FakeProposalRepository
        {
            ResolveTracked = id => id == 11 ? first : id == 12 ? second : null
        };
        var deals = new FakeDealRepository();
        var handler = new AcceptProposalCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            proposals,
            deals);

        var firstResult = await handler.Handle(new AcceptProposalCommand(11), CancellationToken.None);
        var secondResult = await handler.Handle(new AcceptProposalCommand(12), CancellationToken.None);

        Assert.Equal("Accepted", first.Status);
        Assert.Equal("Accepted", second.Status);
        Assert.Equal("Open", request.Status);
        Assert.Equal(2, deals.Added.Count);
        Assert.All(deals.Added, d => Assert.Equal("Active", d.Status));
        Assert.Equal(1, firstResult.DealId);
        Assert.Equal(2, secondResult.DealId);
        Assert.NotEqual(firstResult.DealId, secondResult.DealId);
    }
}

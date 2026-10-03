using Matchi.Application.Features.Businesses.Commands;
using Matchi.Application.Features.Executions.Commands.CompleteServiceExecution;
using Matchi.Application.Features.Executions.Commands.CreateExecutionAssignment;
using Matchi.Application.Features.Matching;
using Matchi.Application.Features.Notifications.Commands;
using Matchi.Application.Features.Notifications.Queries;
using Matchi.Application.Features.Proposals.Commands.AcceptProposal;
using Matchi.Application.Features.Proposals.Commands.CreateProposal;
using Matchi.Application.Features.Proposals.Commands.RejectProposal;
using Matchi.Application.Features.Requests.Commands.CancelRequest;
using Matchi.Application.Notifications;
using Matchi.Domain.Entities;

namespace Matchi.Application.Tests;

public sealed class NotificationWorkflowTests
{
    [Fact]
    public async Task CreateProposal_NotifiesRequestCustomer()
    {
        var customer = new Customer(MarketplaceGraph.CustomerUserId).WithId(1);
        var request = new Request(customer.Id, "Service", "Need work").WithId(8);
        request.Set(nameof(Request.Customer), customer);
        var notifications = TestNotifications.Service();
        var handler = new CreateProposalCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
            new FakeRequestRepository { Owned = request },
            new FakeProviderRepository { Mine = MarketplaceGraph.OwnProvider() },
            new OfferingBusinessRepository(),
            new FakeProposalRepository(),
            notifications,
            TestNotifications.Recipients());

        await handler.Handle(new CreateProposalCommand(8, "Provider", 100m), CancellationToken.None);

        var created = Assert.Single(notifications.Created);
        Assert.Equal(MarketplaceGraph.CustomerUserId, created.UserId);
        Assert.Equal(NotificationTypes.NewProposal, created.Definition.Type);
    }

    [Fact]
    public async Task AcceptProposal_NotifiesOriginator()
    {
        var request = new Request(1, "Service", "Need work").WithId(1);
        request.Set(nameof(Request.Customer), new Customer(MarketplaceGraph.CustomerUserId).WithId(1));
        var proposal = Proposal.ForProvider(request.Id, 5, 100m).WithId(11);
        proposal.Set(nameof(Proposal.Provider), MarketplaceGraph.OwnProvider());
        proposal.Set(nameof(Proposal.Request), request);
        var notifications = TestNotifications.Service();
        var handler = new AcceptProposalCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            new FakeProposalRepository { Tracked = proposal },
            new FakeDealRepository(),
            notifications,
            TestNotifications.Recipients());

        await handler.Handle(new AcceptProposalCommand(11), CancellationToken.None);

        var created = Assert.Single(notifications.Created);
        Assert.Equal(MarketplaceGraph.ProviderUserId, created.UserId);
        Assert.Equal(NotificationTypes.ProposalAccepted, created.Definition.Type);
    }

    [Fact]
    public async Task RejectProposal_NotifiesOriginator()
    {
        var request = new Request(1, "Service", "Need work").WithId(1);
        request.Set(nameof(Request.Customer), new Customer(MarketplaceGraph.CustomerUserId).WithId(1));
        var proposal = Proposal.ForProvider(request.Id, 5, 100m).WithId(11);
        proposal.Set(nameof(Proposal.Provider), MarketplaceGraph.OwnProvider());
        proposal.Set(nameof(Proposal.Request), request);
        var notifications = TestNotifications.Service();
        var handler = new RejectProposalCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            new FakeProposalRepository { Tracked = proposal },
            notifications,
            TestNotifications.Recipients());

        await handler.Handle(new RejectProposalCommand(11), CancellationToken.None);

        var created = Assert.Single(notifications.Created);
        Assert.Equal(MarketplaceGraph.ProviderUserId, created.UserId);
        Assert.Equal(NotificationTypes.ProposalRejected, created.Definition.Type);
    }

    [Fact]
    public async Task CancelRequest_NotifiesOnlyPendingProposalOriginators()
    {
        var customer = new Customer(MarketplaceGraph.CustomerUserId).WithId(1);
        var request = new Request(customer.Id, "Service", "Need work").WithId(1);
        request.Set(nameof(Request.Customer), customer);

        var pending = Proposal.ForProvider(request.Id, MarketplaceGraph.ProviderEntityId, 100m).WithId(1);
        pending.Set(nameof(Proposal.Provider), MarketplaceGraph.OwnProvider());

        var rejected = Proposal.ForProvider(request.Id, MarketplaceGraph.OtherProviderEntityId, 90m).WithId(2);
        rejected.Set(nameof(Proposal.Provider), MarketplaceGraph.OtherProvider());
        rejected.Reject();

        var proposals = new FakeProposalRepository();
        proposals.OwnedRequestProposals.Add(pending);
        proposals.OwnedRequestProposals.Add(rejected);

        var notifications = TestNotifications.Service();
        var handler = new CancelRequestCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            new FakeRequestRepository { Owned = request },
            new FakeDealRepository { HasActiveDeal = false },
            proposals,
            notifications,
            TestNotifications.Recipients());

        await handler.Handle(new CancelRequestCommand(1), CancellationToken.None);

        var created = Assert.Single(notifications.Created);
        Assert.Equal(MarketplaceGraph.ProviderUserId, created.UserId);
        Assert.Equal(NotificationTypes.RequestCancelled, created.Definition.Type);
        Assert.DoesNotContain(notifications.Created, x => x.UserId == MarketplaceGraph.OtherProviderUserId);
    }

    [Fact]
    public async Task InviteProvider_NotifiesInvitedProvider()
    {
        var business = new Business(MarketplaceGraph.BusinessOwnerUserId, "Biz").WithId(7);
        var businesses = new OfferingBusinessRepository();
        businesses.Owned.Add(business);
        businesses.ExistingProviderIds.Add(MarketplaceGraph.OtherProviderEntityId);
        var provider = MarketplaceGraph.OtherProvider();
        var notifications = TestNotifications.Service();
        var handler = new InviteProviderCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.BusinessOwnerUserId),
            businesses,
            new StubUserRepository(new User("09120000000").WithId(1)),
            new FakeProviderRepository { Mine = provider },
            notifications);

        await handler.Handle(
            new InviteProviderCommand(7, MarketplaceGraph.OtherProviderEntityId, null, "Member"),
            CancellationToken.None);

        var created = Assert.Single(notifications.Created);
        Assert.Equal(MarketplaceGraph.OtherProviderUserId, created.UserId);
        Assert.Equal(NotificationTypes.InvitationReceived, created.Definition.Type);
    }

    [Fact]
    public async Task AcceptInvitation_NotifiesBusinessOwner()
    {
        var business = new Business(MarketplaceGraph.BusinessOwnerUserId, "Biz").WithId(7);
        var provider = MarketplaceGraph.OtherProvider();
        var membership = new BusinessProvider(business.Id, provider.Id).WithId(4);
        membership.UpdateMembership("Member", "Pending");
        membership.Set(nameof(BusinessProvider.Provider), provider);
        membership.Set(nameof(BusinessProvider.Business), business);
        var notifications = TestNotifications.Service();
        var handler = new AcceptBusinessMembershipCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.OtherProviderUserId),
            new OfferingBusinessRepository { Membership = membership },
            notifications);

        var result = await handler.Handle(new AcceptBusinessMembershipCommand(4), CancellationToken.None);

        Assert.True(result);
        var created = Assert.Single(notifications.Created);
        Assert.Equal(MarketplaceGraph.BusinessOwnerUserId, created.UserId);
        Assert.Equal(NotificationTypes.InvitationAccepted, created.Definition.Type);
    }

    [Fact]
    public async Task RejectInvitation_NotifiesBusinessOwner()
    {
        var business = new Business(MarketplaceGraph.BusinessOwnerUserId, "Biz").WithId(7);
        var provider = MarketplaceGraph.OtherProvider();
        var membership = new BusinessProvider(business.Id, provider.Id).WithId(4);
        membership.UpdateMembership("Member", "Pending");
        membership.Set(nameof(BusinessProvider.Provider), provider);
        membership.Set(nameof(BusinessProvider.Business), business);
        var notifications = TestNotifications.Service();
        var handler = new RejectBusinessMembershipCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.OtherProviderUserId),
            new OfferingBusinessRepository { Membership = membership },
            notifications);

        var result = await handler.Handle(new RejectBusinessMembershipCommand(4), CancellationToken.None);

        Assert.True(result);
        var created = Assert.Single(notifications.Created);
        Assert.Equal(MarketplaceGraph.BusinessOwnerUserId, created.UserId);
        Assert.Equal(NotificationTypes.InvitationRejected, created.Definition.Type);
    }

    [Fact]
    public async Task AssignProvider_NotifiesAssignedProvider()
    {
        var deal = MarketplaceGraph.BusinessDeal();
        var execution = ServiceExecution.Create(deal.Id, deal.Proposal.BusinessId).WithId(3);
        execution.Set(nameof(ServiceExecution.Deal), deal);
        var notifications = TestNotifications.Service();
        var providers = new FakeProviderRepository { Mine = MarketplaceGraph.OtherProvider() };
        var handler = new CreateExecutionAssignmentCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.BusinessOwnerUserId),
            new FakeServiceExecutionRepository { Tracked = execution },
            new FakeExecutionAssignmentRepository { HasPrimary = false, HasMembership = true },
            providers,
            notifications);

        await handler.Handle(
            new CreateExecutionAssignmentCommand(3, MarketplaceGraph.OtherProviderEntityId, "Technician", false),
            CancellationToken.None);

        var created = Assert.Single(notifications.Created);
        Assert.Equal(MarketplaceGraph.OtherProviderUserId, created.UserId);
        Assert.Equal(NotificationTypes.ProviderAssigned, created.Definition.Type);
    }

    [Fact]
    public async Task CompleteExecution_NotifiesCustomer()
    {
        var deal = MarketplaceGraph.ProviderDeal();
        var execution = ServiceExecution.Create(deal.Id, null).WithId(1);
        execution.Set(nameof(ServiceExecution.Deal), deal);
        execution.Start();
        var notifications = TestNotifications.Service();
        var handler = new CompleteServiceExecutionCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.ProviderUserId),
            new FakeServiceExecutionRepository { Tracked = execution },
            notifications,
            TestNotifications.Recipients());

        await handler.Handle(new CompleteServiceExecutionCommand(1), CancellationToken.None);

        Assert.Contains(
            notifications.Created,
            x => x.UserId == MarketplaceGraph.CustomerUserId && x.Definition.Type == NotificationTypes.ExecutionCompleted);
        Assert.DoesNotContain(notifications.Created, x => x.UserId == MarketplaceGraph.ProviderUserId);
    }

    [Fact]
    public async Task GetMatches_DoesNotCreateNotifications()
    {
        var customer = new Customer(MarketplaceGraph.CustomerUserId).WithId(1);
        var request = new Request(customer.Id, "Service", "Need work").WithId(1);
        request.Set(nameof(Request.Customer), customer);
        var matching = new FakeMatchingReadRepository();
        var handler = new GetRequestMatchesQueryHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            new FakeRequestRepository { Owned = request },
            matching);

        await handler.Handle(new GetRequestMatchesQuery(1), CancellationToken.None);

        Assert.Equal(1, matching.ProviderMatchQueryCount);
    }
}

public sealed class NotificationQueryCommandTests
{
    [Fact]
    public async Task UserCannotReadAnotherUsersNotification()
    {
        var repo = new FakeNotificationRepository();
        await repo.AddAsync(new Notification(7, NotificationTypes.NewProposal, "t", "m").WithId(3));
        var handler = new MarkNotificationAsReadCommandHandler(new FakeCurrentUser(1), repo);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new MarkNotificationAsReadCommand(3), CancellationToken.None));
        Assert.False(repo.Items[0].IsRead);
    }

    [Fact]
    public async Task MarkRead_OnlyAffectsCurrentUserNotification()
    {
        var repo = new FakeNotificationRepository();
        var mine = new Notification(1, NotificationTypes.NewProposal, "t", "m").WithId(3);
        var other = new Notification(2, NotificationTypes.NewProposal, "t", "m").WithId(4);
        await repo.AddAsync(mine);
        await repo.AddAsync(other);
        var handler = new MarkNotificationAsReadCommandHandler(new FakeCurrentUser(1), repo);

        await handler.Handle(new MarkNotificationAsReadCommand(3), CancellationToken.None);

        Assert.True(mine.IsRead);
        Assert.False(other.IsRead);
    }

    [Fact]
    public async Task MarkAllRead_OnlyAffectsCurrentUser()
    {
        var repo = new FakeNotificationRepository();
        var mine = new Notification(1, NotificationTypes.NewProposal, "t", "m").WithId(3);
        var other = new Notification(2, NotificationTypes.NewProposal, "t", "m").WithId(4);
        await repo.AddAsync(mine);
        await repo.AddAsync(other);
        var handler = new MarkAllNotificationsAsReadCommandHandler(new FakeCurrentUser(1), repo);

        var updated = await handler.Handle(new MarkAllNotificationsAsReadCommand(), CancellationToken.None);

        Assert.Equal(1, updated);
        Assert.True(mine.IsRead);
        Assert.False(other.IsRead);
    }

    [Fact]
    public async Task Notify_IsIdempotentForSameReferenceKey()
    {
        var repo = new FakeNotificationRepository();
        var service = new NotificationService(repo);
        var definition = NotificationCatalog.NewProposal(8, 11);

        await service.NotifyAsync(1, definition);
        await service.NotifyAsync(1, definition);

        Assert.Single(repo.Items);
        Assert.Equal(1, repo.SaveCount);
    }

    [Fact]
    public async Task List_DoesNotExposeAnotherUsersNotifications()
    {
        var repo = new FakeNotificationRepository();
        await repo.AddAsync(new Notification(7, NotificationTypes.NewProposal, "t", "m").WithId(3));
        var handler = new GetMyNotificationsQueryHandler(new FakeCurrentUser(1), repo);

        var result = await handler.Handle(new GetMyNotificationsQuery(), CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.UnreadCount);
    }

    [Fact]
    public async Task List_DoesNotCreateNotifications()
    {
        var repo = new FakeNotificationRepository();
        var handler = new GetMyNotificationsQueryHandler(new FakeCurrentUser(1), repo);

        var result = await handler.Handle(new GetMyNotificationsQuery(), CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Empty(repo.Items);
        Assert.Equal(0, repo.SaveCount);
    }
}

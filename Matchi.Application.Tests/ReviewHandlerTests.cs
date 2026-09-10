using FluentValidation;
using Matchi.Application.Common;
using Matchi.Application.Features.Reviews.Commands.CreateReview;
using Matchi.Application.Features.Reviews.Queries.GetBusinessReviews;
using Matchi.Application.Features.Reviews.Queries.GetProviderReviews;
using Matchi.Domain.Entities;

namespace Matchi.Application.Tests;

public sealed class CreateReviewCommandHandlerTests
{
    private static Deal CompletedProviderDeal()
    {
        var deal = MarketplaceGraph.ProviderDeal();
        var execution = ServiceExecution.Create(deal.Id, null).WithId(1);
        execution.Start();
        execution.Complete();
        deal.ServiceExecutions.Add(execution);
        return deal;
    }

    [Fact]
    public async Task Customer_CanCreateValidProviderReview()
    {
        var reviews = new FakeReviewRepository { Deal = CompletedProviderDeal() };
        var handler = new CreateReviewCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            reviews);

        var id = await handler.Handle(
            new CreateReviewCommand(1, 5, "Great", null, 5),
            CancellationToken.None);

        Assert.Equal(1, id);
        Assert.Equal(5, reviews.Added[0].ProviderId);
        Assert.Equal((byte)5, reviews.Added[0].Rating);
    }

    [Fact]
    public async Task NonOwnerCustomer_IsRejected()
    {
        var handler = new CreateReviewCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.OtherUserId),
            new FakeReviewRepository { Deal = CompletedProviderDeal() });

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new CreateReviewCommand(1, 5, null, null, 5), CancellationToken.None));
    }

    [Fact]
    public async Task InvalidRating_IsRejectedByValidator()
    {
        var validator = new CreateReviewCommandValidator();
        var result = await validator.ValidateAsync(new CreateReviewCommand(1, 0, null, null, 5));
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task InvalidTarget_IsRejected()
    {
        var handler = new CreateReviewCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            new FakeReviewRepository { Deal = CompletedProviderDeal() });

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateReviewCommand(1, 4, null, 99, null), CancellationToken.None));
        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateReviewCommand(1, 4, null, null, 99), CancellationToken.None));
    }

    [Fact]
    public async Task DuplicateReview_IsRejected()
    {
        var handler = new CreateReviewCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            new FakeReviewRepository { Deal = CompletedProviderDeal(), Exists = true });

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateReviewCommand(1, 5, null, null, 5), CancellationToken.None));
    }

    [Fact]
    public async Task DuplicateReviewRace_IsConflict()
    {
        var handler = new CreateReviewCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            new FakeReviewRepository
            {
                Deal = CompletedProviderDeal(),
                SaveException = new ConflictException("A review already exists for this target.")
            });

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new CreateReviewCommand(1, 5, null, null, 5), CancellationToken.None));
    }

    [Fact]
    public async Task ServiceReview_RequiresCompletedExecution()
    {
        var handler = new CreateReviewCommandHandler(
            new FakeCurrentUser(MarketplaceGraph.CustomerUserId),
            new FakeReviewRepository { Deal = MarketplaceGraph.ProviderDeal() });

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateReviewCommand(1, 5, null, null, 5), CancellationToken.None));
    }
}

public sealed class GetReviewsQueryHandlerTests
{
    [Fact]
    public async Task ProviderReviews_AreReturnedAndDeletedExcluded()
    {
        var live = Review.Create(1, 2, 5, "ok", null, 8).WithId(1);
        var deleted = Review.Create(1, 2, 4, "gone", null, 8).WithId(2);
        deleted.SoftDelete();
        var other = Review.Create(3, 2, 3, "else", null, 9).WithId(3);

        var handler = new GetProviderReviewsQueryHandler(new FakeReviewRepository
        {
            ProviderReviews = { live, deleted, other }
        });

        var result = (await handler.Handle(new GetProviderReviewsQuery(8), CancellationToken.None)).ToList();
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Provider", result[0].TargetType);
        Assert.Equal(8, result[0].TargetId);
    }

    [Fact]
    public async Task BusinessReviews_AreReturnedAndDeletedExcluded()
    {
        var live = Review.Create(1, 2, 5, "ok", 7, null).WithId(1);
        var deleted = Review.Create(2, 2, 2, "gone", 7, null).WithId(2);
        deleted.SoftDelete();

        var handler = new GetBusinessReviewsQueryHandler(new FakeReviewRepository
        {
            BusinessReviews = { live, deleted }
        });

        var result = (await handler.Handle(new GetBusinessReviewsQuery(7), CancellationToken.None)).ToList();
        Assert.Single(result);
        Assert.Equal("Business", result[0].TargetType);
        Assert.Equal(7, result[0].TargetId);
    }

    [Fact]
    public async Task DualTargetLegacyRow_DoesNotThrowOnProviderOrBusinessLists()
    {
        var dual = Review.Create(1, 2, 5, "legacy", null, 8).WithId(1);
        dual.Set(nameof(Review.BusinessId), 7L);

        var providerHandler = new GetProviderReviewsQueryHandler(new FakeReviewRepository
        {
            ProviderReviews = { dual }
        });
        var businessHandler = new GetBusinessReviewsQueryHandler(new FakeReviewRepository
        {
            BusinessReviews = { dual }
        });

        var provider = (await providerHandler.Handle(new GetProviderReviewsQuery(8), CancellationToken.None)).ToList();
        var business = (await businessHandler.Handle(new GetBusinessReviewsQuery(7), CancellationToken.None)).ToList();

        Assert.Single(provider);
        Assert.Equal("Provider", provider[0].TargetType);
        Assert.Equal(8, provider[0].TargetId);
        Assert.Single(business);
        Assert.Equal("Business", business[0].TargetType);
        Assert.Equal(7, business[0].TargetId);
    }
}

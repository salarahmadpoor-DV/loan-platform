using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Reviews.Commands.CreateReview;

public sealed record CreateReviewCommand(
    long DealId,
    byte Rating,
    string? Comment = null,
    long? BusinessId = null,
    long? ProviderId = null) : IRequest<long>;

public sealed class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.DealId).GreaterThan(0);
        RuleFor(x => x.Rating).InclusiveBetween((byte)1, (byte)5);
        RuleFor(x => x.Comment).MaximumLength(2000);
        RuleFor(x => x)
            .Must(x => (x.BusinessId is null) != (x.ProviderId is null))
            .WithMessage("A review must target exactly one of Business or Provider.");
        RuleFor(x => x.BusinessId).GreaterThan(0).When(x => x.BusinessId is not null);
        RuleFor(x => x.ProviderId).GreaterThan(0).When(x => x.ProviderId is not null);
    }
}

public sealed class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, long>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IReviewRepository _reviews;

    public CreateReviewCommandHandler(
        ICurrentUserService currentUserService,
        IReviewRepository reviews)
    {
        _currentUserService = currentUserService;
        _reviews = reviews;
    }

    public async Task<long> Handle(CreateReviewCommand command, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var deal = await _reviews.GetCustomerDealGraphAsync(command.DealId, userId, cancellationToken);
        if (deal is null)
            throw new KeyNotFoundException("Deal was not found.");

        if (!string.Equals(deal.Status, "Active", StringComparison.Ordinal))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("dealId", "Only an active deal can be reviewed.")
            });
        }

        if (deal.Request.RequestType is "Service" or "Hybrid"
            && !deal.ServiceExecutions.Any(e => string.Equals(e.Status, "Completed", StringComparison.Ordinal)))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("dealId", "A service review requires a completed execution.")
            });
        }

        if (command.BusinessId is not null)
        {
            if (deal.Proposal.BusinessId != command.BusinessId)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("businessId", "Review target must be the proposal business.")
                });
            }
        }
        else
        {
            var providerId = command.ProviderId!.Value;
            var isProposalProvider = deal.Proposal.ProviderId == providerId;
            var isAssignedProvider = deal.ServiceExecutions.Any(e =>
                e.Assignments.Any(a => a.ProviderId == providerId));

            if (!isProposalProvider && !isAssignedProvider)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("providerId", "Review target must be the proposal or assigned provider.")
                });
            }
        }

        if (await _reviews.ExistsActiveTargetAsync(
                deal.Id,
                deal.CustomerId,
                command.BusinessId,
                command.ProviderId,
                cancellationToken))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("dealId", "A review already exists for this target.")
            });
        }

        Review review;
        try
        {
            review = Review.Create(
                deal.Id,
                deal.CustomerId,
                command.Rating,
                command.Comment,
                command.BusinessId,
                command.ProviderId);
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure("rating", ex.Message) });
        }

        _reviews.Add(review);
        await _reviews.SaveChangesAsync(cancellationToken);

        return review.Id;
    }
}

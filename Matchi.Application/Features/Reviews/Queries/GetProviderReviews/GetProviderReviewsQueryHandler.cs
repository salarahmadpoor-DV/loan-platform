using Matchi.Application.Features.Reviews.Queries;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Reviews.Queries.GetProviderReviews;

public sealed class GetProviderReviewsQueryHandler : IRequestHandler<GetProviderReviewsQuery, IEnumerable<ReviewDto>>
{
    private readonly IReviewRepository _reviews;

    public GetProviderReviewsQueryHandler(IReviewRepository reviews)
    {
        _reviews = reviews;
    }

    public async Task<IEnumerable<ReviewDto>> Handle(
        GetProviderReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await _reviews.ListByProviderAsync(request.ProviderId, cancellationToken);
        return reviews.Select(r => new ReviewDto(
            r.Id,
            r.ProviderId!.Value,
            "Provider",
            r.Rating,
            r.Comment));
    }
}

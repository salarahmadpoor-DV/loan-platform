using Matchi.Application.Features.Reviews.Queries;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Reviews.Queries.GetBusinessReviews;

public sealed class GetBusinessReviewsQueryHandler : IRequestHandler<GetBusinessReviewsQuery, IEnumerable<ReviewDto>>
{
    private readonly IReviewRepository _reviews;

    public GetBusinessReviewsQueryHandler(IReviewRepository reviews)
    {
        _reviews = reviews;
    }

    public async Task<IEnumerable<ReviewDto>> Handle(
        GetBusinessReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await _reviews.ListByBusinessAsync(request.BusinessId, cancellationToken);
        return reviews.Select(r => new ReviewDto(
            r.Id,
            r.BusinessId!.Value,
            "Business",
            r.Rating,
            r.Comment));
    }
}

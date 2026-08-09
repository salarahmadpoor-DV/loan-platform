using Matchi.Application.Features.Reviews.Queries;
using MediatR;

namespace Matchi.Application.Features.Reviews.Queries.GetBusinessReviews;

public sealed class GetBusinessReviewsQueryHandler : IRequestHandler<GetBusinessReviewsQuery, IEnumerable<ReviewDto>>
{
    public Task<IEnumerable<ReviewDto>> Handle(
        GetBusinessReviewsQuery request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult((IEnumerable<ReviewDto>)Array.Empty<ReviewDto>());
    }
}

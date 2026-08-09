using Matchi.Application.Features.Reviews.Queries;
using MediatR;

namespace Matchi.Application.Features.Reviews.Queries.GetProviderReviews;

public sealed class GetProviderReviewsQueryHandler : IRequestHandler<GetProviderReviewsQuery, IEnumerable<ReviewDto>>
{
    public Task<IEnumerable<ReviewDto>> Handle(
        GetProviderReviewsQuery request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult((IEnumerable<ReviewDto>)Array.Empty<ReviewDto>());
    }
}

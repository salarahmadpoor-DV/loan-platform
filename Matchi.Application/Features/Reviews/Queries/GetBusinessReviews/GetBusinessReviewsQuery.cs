using MediatR;

namespace Matchi.Application.Features.Reviews.Queries.GetBusinessReviews;

public sealed record GetBusinessReviewsQuery(long BusinessId) : IRequest<IEnumerable<ReviewDto>>;

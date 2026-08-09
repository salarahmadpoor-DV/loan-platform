using MediatR;

namespace Matchi.Application.Features.Reviews.Queries.GetProviderReviews;

public sealed record GetProviderReviewsQuery(long ProviderId) : IRequest<IEnumerable<ReviewDto>>;

using MediatR;

namespace Matchi.Application.Features.Reviews.Commands.CreateReview;

public sealed record CreateReviewCommand(long IntroductionId, string TargetType, long TargetId, int Rating, string? Comment) : IRequest<long>;

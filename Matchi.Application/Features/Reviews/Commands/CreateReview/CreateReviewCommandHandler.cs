using MediatR;

namespace Matchi.Application.Features.Reviews.Commands.CreateReview;

public sealed class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, long>
{
    public Task<long> Handle(
        CreateReviewCommand request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(1L);
    }
}

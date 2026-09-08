using MediatR;
using Matchi.Application.Features.Reviews.Queries.GetBusinessReviews;
using Matchi.Application.Features.Reviews.Queries.GetProviderReviews;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("providers/{providerId}/reviews")]
    public async Task<IActionResult> GetProviderReviews(long providerId, CancellationToken cancellationToken = default)
    {
        var reviews = await _mediator.Send(new GetProviderReviewsQuery(providerId), cancellationToken);
        return Ok(reviews);
    }

    [HttpGet("businesses/{businessId}/reviews")]
    public async Task<IActionResult> GetBusinessReviews(long businessId, CancellationToken cancellationToken = default)
    {
        var reviews = await _mediator.Send(new GetBusinessReviewsQuery(businessId), cancellationToken);
        return Ok(reviews);
    }
}

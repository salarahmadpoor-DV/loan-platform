using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Matchi.Application.Features.Reviews.Commands.CreateReview;
using Matchi.Application.Features.Reviews.Queries.GetBusinessReviews;
using Matchi.Application.Features.Reviews.Queries.GetProviderReviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public record CreateReviewDto(string TargetType, long TargetId, int Rating, string? Comment);

        [HttpPost("introductions/{introductionId}/reviews")]
        [Authorize]
        public async Task<IActionResult> Create(long introductionId, [FromBody] CreateReviewDto dto, CancellationToken cancellationToken = default)
        {
            var reviewId = await _mediator.Send(new CreateReviewCommand(introductionId, dto.TargetType, dto.TargetId, dto.Rating, dto.Comment), cancellationToken);
            return Created("", new { reviewId });
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
}

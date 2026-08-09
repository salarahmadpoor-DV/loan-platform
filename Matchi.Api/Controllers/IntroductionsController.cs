using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Matchi.Application.Features.Introductions.Commands.AssignIntroduction;
using Matchi.Application.Features.Introductions.Commands.ConfirmIntroduction;
using Matchi.Application.Features.Introductions.Commands;
using Matchi.Application.Features.Introductions.Queries.GetIntroductionsByRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntroductionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IntroductionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public record CreateIntroductionDto(long RequestId, string TargetType, long TargetId, string? Source);
        public record AssignDto(long ProviderId);
        public record ConfirmDto(string Status);

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateIntroductionDto dto, CancellationToken cancellationToken = default)
        {
            var introductionId = await _mediator.Send(new CreateIntroductionCommand(dto.RequestId, dto.TargetType, dto.TargetId, dto.Source), cancellationToken);
            return CreatedAtAction(nameof(GetByRequest), new { requestId = dto.RequestId }, new { introductionId, status = "Created" });
        }

        [HttpGet("request/{requestId}")]
        [Authorize]
        public async Task<IActionResult> GetByRequest(long requestId, CancellationToken cancellationToken = default)
        {
            var introductions = await _mediator.Send(new GetIntroductionsByRequestQuery(requestId), cancellationToken);
            return Ok(introductions);
        }

        [HttpPut("{introductionId}/assign")]
        [Authorize]
        public async Task<IActionResult> Assign(long introductionId, [FromBody] AssignDto dto, CancellationToken cancellationToken = default)
        {
            var success = await _mediator.Send(new AssignIntroductionCommand(introductionId, dto.ProviderId), cancellationToken);
            if (!success)
                return NotFound();

            return Ok(new { introductionId, assignedProviderId = dto.ProviderId, status = "Assigned" });
        }

        [HttpPut("{introductionId}/confirm")]
        [Authorize]
        public async Task<IActionResult> Confirm(long introductionId, [FromBody] ConfirmDto dto, CancellationToken cancellationToken = default)
        {
            var success = await _mediator.Send(new ConfirmIntroductionCommand(introductionId, dto.Status), cancellationToken);
            if (!success)
                return NotFound();

            return Ok(new { introductionId, status = dto.Status });
        }
    }
}

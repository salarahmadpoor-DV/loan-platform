using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Matchi.Application.Features.Requests.Commands;
using Matchi.Application.Features.Requests.Queries.GetServiceRequestById;
using Matchi.Application.Features.Requests.Queries.GetServiceRequestsByUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly Matchi.Application.Common.Interfaces.ICurrentUserService _currentUserService;

        public RequestsController(
            IMediator mediator,
            Matchi.Application.Common.Interfaces.ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        public record AnswerDto(
            long QuestionId,
            long? OptionId,
            string? Text);

        public record CreateRequestDto(
            long ServiceId,
            string Title,
            string? Description,
            RequestLocationDto Location,
            PreferredTimeDto? PreferredTime,
            IEnumerable<AnswerDto>? Answers,
            IEnumerable<string>? Attachments);

        public record RequestLocationDto(
            double Lat,
            double Lng,
            string? Address);

        public record PreferredTimeDto(
            DateTime? From,
            DateTime? To);

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateRequestDto dto, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized();

            var answers = dto.Answers?.Select(a => new RequestAnswerDto(a.QuestionId, a.OptionId, a.Text));
            var cmd = new CreateServiceRequestCommand(userId.Value, dto.ServiceId, dto.Title, dto.Description, dto.Location?.Lat, dto.Location?.Lng, answers);
            var id = await _mediator.Send(cmd, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { requestId = id }, new { requestId = id, status = "Created", matchingStatus = "Pending" });
        }

        [HttpGet("{requestId}")]
        [Authorize]
        public async Task<IActionResult> GetById(long requestId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetServiceRequestByIdQuery(requestId), cancellationToken);
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetByUser(long userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetServiceRequestsByUserQuery(userId), cancellationToken);
            return Ok(new { page, pageSize, items = result });
        }
    }
}

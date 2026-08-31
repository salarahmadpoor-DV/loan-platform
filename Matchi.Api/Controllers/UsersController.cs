using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Matchi.Application.Features.Users.Commands;
using Matchi.Application.Features.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly Matchi.Application.Common.Interfaces.ICurrentUserService _currentUserService;

        public UsersController(
            IMediator mediator,
            Matchi.Application.Common.Interfaces.ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        public record UpdateUserRequest(string? Name, UserLocationDto? Location);
        public record UserLocationDto(double Lat, double Lng, string? Address);

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me(CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized();

            var user = await _mediator.Send(new GetCurrentUserQuery(userId.Value), cancellationToken);
            if (user is null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized();

            var success = await _mediator.Send(new UpdateUserProfileCommand(userId.Value, request.Name, request.Location?.Lat, request.Location?.Lng, request.Location?.Address), cancellationToken);
            if (!success)
                return NotFound();

            return Ok(new { success = true });
        }
        [HttpGet("request-view-test")]
        [Authorize(Policy = "REQUEST_VIEW")]
        public IActionResult RequestViewTest()
        {
            return Ok(new
            {
                message = "REQUEST_VIEW permission granted"
            });
        }
    }
}

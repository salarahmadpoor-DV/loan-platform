using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Matchi.Application.Features.Businesses.Commands;
using Matchi.Application.Features.Businesses.Queries.GetBusinesses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BusinessesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public record CreateBusinessDto(string Name, string? Address, LocationDto? Location, string? OwnerContact);
        public record LocationDto(double Lat, double Lng, string? Address);
        public record InviteDto(long ProviderId, string? Role);

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateBusinessDto dto, CancellationToken cancellationToken = default)
        {
            var businessId = await _mediator.Send(new CreateBusinessCommand(dto.Name, dto.Address, dto.Location?.Lat, dto.Location?.Lng), cancellationToken);
            return CreatedAtAction(nameof(List), new { businessId }, new { businessId });
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var businesses = await _mediator.Send(new GetBusinessesQuery(page, pageSize), cancellationToken);
            return Ok(new { page, pageSize, items = businesses });
        }

        [HttpPost("{businessId}/invite-provider")]
        [Authorize]
        public async Task<IActionResult> InviteProvider(long businessId, [FromBody] InviteDto dto, CancellationToken cancellationToken = default)
        {
            var inviteId = await _mediator.Send(new InviteProviderCommand(businessId, dto.ProviderId, dto.Role), cancellationToken);
            return Accepted(new { inviteId, status = "Pending" });
        }

        [HttpPut("business-providers/{businessProviderId}/accept")]
        [Authorize]
        public async Task<IActionResult> AcceptMembership(long businessProviderId, CancellationToken cancellationToken = default)
        {
            var success = await _mediator.Send(new AcceptBusinessMembershipCommand(businessProviderId), cancellationToken);
            if (!success)
                return NotFound();

            return Ok(new { businessProviderId, status = "Active" });
        }
    }
}

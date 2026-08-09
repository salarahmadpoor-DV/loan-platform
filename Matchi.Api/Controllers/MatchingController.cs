using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Matchi.Application.Features.Matching.Commands;
using Matchi.Application.Features.Matching.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers
{
    [ApiController]
    [Route("api/requests/{requestId}/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MatchesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public record MatchRequestDto(string? Method = "basic");

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Match(Guid requestId, [FromBody] MatchRequestDto? dto, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new MatchRequestCommand(requestId, dto?.Method), cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMatches(Guid requestId, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetMatchesQuery(requestId), cancellationToken);
            return Ok(result);
        }
    }
}

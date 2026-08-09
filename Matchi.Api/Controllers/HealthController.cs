using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Matchi.Application.Features.Health.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HealthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetHealthStatusQuery(), cancellationToken);
            return Ok(result);
        }
    }
}

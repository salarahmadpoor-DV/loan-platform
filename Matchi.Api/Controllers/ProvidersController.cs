using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Matchi.Application.Features.Providers.Commands;
using Matchi.Application.Features.Providers.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvidersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProvidersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProviderCommand command, CancellationToken cancellationToken = default)
        {
            var providerId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new { providerId }, new { providerId });
        }

        [HttpGet("{providerId}")]
        public async Task<IActionResult> Get(long providerId, CancellationToken cancellationToken = default)
        {
            var provider = await _mediator.Send(new GetProviderByIdQuery(providerId), cancellationToken);
            if (provider is null)
                return NotFound();

            return Ok(provider);
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] long? serviceId, [FromQuery] double? lat = null, [FromQuery] double? lng = null, [FromQuery] int radiusKm = 10, [FromQuery] string? sort = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var providers = await _mediator.Send(new SearchProvidersQuery(serviceId, lat, lng, radiusKm, sort, page, pageSize), cancellationToken);
            return Ok(new { page, pageSize, items = providers });
        }
    }
}

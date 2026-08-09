using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Matchi.Application.Features.Services.Queries.GetServiceById;
using Matchi.Application.Features.Services.Queries.GetServiceCategories;
using Matchi.Application.Features.Services.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken = default)
        {
            var categories = await _mediator.Send(new GetServiceCategoriesQuery(), cancellationToken);
            return Ok(categories);
        }

        [HttpGet]
        public async Task<IActionResult> GetServices([FromQuery] long? categoryId, [FromQuery] string? q = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var services = await _mediator.Send(new GetServicesQuery(categoryId), cancellationToken);
            return Ok(new { page, pageSize, items = services });
        }

        [HttpGet("{serviceId}")]
        public async Task<IActionResult> GetServiceDetails(long serviceId, CancellationToken cancellationToken = default)
        {
            var item = await _mediator.Send(new GetServiceByIdQuery(serviceId), cancellationToken);
            if (item is null) return NotFound();
            return Ok(item);
        }
    }
}

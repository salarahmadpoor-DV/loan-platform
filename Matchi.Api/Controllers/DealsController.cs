using Matchi.Application.Features.Deals.Queries.GetDealById;
using Matchi.Application.Features.Deals.Queries.GetMyDeals;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DealsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DealsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken = default)
    {
        var deals = await _mediator.Send(new GetMyDealsQuery(), cancellationToken);
        return Ok(deals);
    }

    [HttpGet("{dealId:long}")]
    public async Task<IActionResult> Get(long dealId, CancellationToken cancellationToken = default)
    {
        var deal = await _mediator.Send(new GetDealByIdQuery(dealId), cancellationToken);
        if (deal is null)
            return NotFound();

        return Ok(deal);
    }
}

using Matchi.Application.Features.Providers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api/provider")]
[Authorize(Policy = "ProviderWorkspace")]
public class ProviderMarketplaceController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProviderMarketplaceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("requests")]
    public async Task<IActionResult> GetRequestInbox(CancellationToken cancellationToken = default)
    {
        var items = await _mediator.Send(new GetProviderRequestInboxQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpGet("proposals")]
    public async Task<IActionResult> GetMyProposals(CancellationToken cancellationToken = default)
    {
        var items = await _mediator.Send(new GetMyProviderProposalsQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpGet("deals")]
    public async Task<IActionResult> GetMyDeals(CancellationToken cancellationToken = default)
    {
        var items = await _mediator.Send(new GetMyProviderDealsQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpGet("executions")]
    public async Task<IActionResult> GetMyExecutions(CancellationToken cancellationToken = default)
    {
        var items = await _mediator.Send(new GetMyProviderExecutionsQuery(), cancellationToken);
        return Ok(items);
    }
}

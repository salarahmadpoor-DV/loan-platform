using Matchi.Application.Features.Deals.Queries.GetDealById;
using Matchi.Application.Features.Deals.Queries.GetMyDeals;
using Matchi.Application.Features.Executions.Commands.CreateServiceExecution;
using Matchi.Application.Features.Executions.Queries.GetDealExecutions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

    [HttpGet("{dealId:long}/executions")]
    public async Task<IActionResult> GetExecutions(long dealId, CancellationToken cancellationToken = default)
    {
        var executions = await _mediator.Send(new GetDealExecutionsQuery(dealId), cancellationToken);
        return Ok(executions);
    }

    [HttpPost("{dealId:long}/executions")]
    public async Task<IActionResult> CreateExecution(
        long dealId,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] CreateServiceExecutionBody? body,
        CancellationToken cancellationToken = default)
    {
        var executionId = await _mediator.Send(
            new CreateServiceExecutionCommand(
                dealId,
                body?.ScheduledDate,
                body?.ScheduledTimeFrom,
                body?.ScheduledTimeTo),
            cancellationToken);

        return CreatedAtAction(
            nameof(ExecutionsController.Get),
            "Executions",
            new { executionId },
            new { executionId });
    }
}

public sealed record CreateServiceExecutionBody(
    DateOnly? ScheduledDate = null,
    TimeSpan? ScheduledTimeFrom = null,
    TimeSpan? ScheduledTimeTo = null);


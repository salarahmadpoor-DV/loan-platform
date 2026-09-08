using Matchi.Application.Features.Matching;
using Matchi.Application.Features.Proposals.Commands.CreateProposal;
using Matchi.Application.Features.Proposals.Queries.GetRequestProposals;
using Matchi.Application.Features.Requests.Commands.CancelRequest;
using Matchi.Application.Features.Requests.Commands.CreateRequest;
using Matchi.Application.Features.Requests.Commands.DeleteRequest;
using Matchi.Application.Features.Requests.Commands.UpdateRequest;
using Matchi.Application.Features.Requests.Queries.GetMyRequests;
using Matchi.Application.Features.Requests.Queries.GetRequestById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        var requestId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Get), new { requestId }, new { requestId });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken = default)
    {
        var requests = await _mediator.Send(new GetMyRequestsQuery(), cancellationToken);
        return Ok(requests);
    }

    [HttpGet("{requestId:long}")]
    public async Task<IActionResult> Get(long requestId, CancellationToken cancellationToken = default)
    {
        var request = await _mediator.Send(new GetRequestByIdQuery(requestId), cancellationToken);
        if (request is null)
            return NotFound();

        return Ok(request);
    }

    [HttpGet("{requestId:long}/matches")]
    public async Task<IActionResult> GetMatches(long requestId, CancellationToken cancellationToken = default)
    {
        var matches = await _mediator.Send(new GetRequestMatchesQuery(requestId), cancellationToken);
        return Ok(matches);
    }

    [HttpGet("{requestId:long}/proposals")]
    public async Task<IActionResult> GetProposals(long requestId, CancellationToken cancellationToken = default)
    {
        var proposals = await _mediator.Send(new GetRequestProposalsQuery(requestId), cancellationToken);
        return Ok(proposals);
    }

    [HttpPost("{requestId:long}/proposals")]
    public async Task<IActionResult> CreateProposal(
        long requestId,
        [FromBody] CreateProposalBody body,
        CancellationToken cancellationToken = default)
    {
        var proposalId = await _mediator.Send(
            new CreateProposalCommand(
                requestId,
                body.ProposerType,
                body.TotalPrice,
                body.DeliveryFee,
                body.Message,
                body.ProposedDate,
                body.ProposedTimeFrom,
                body.ProposedTimeTo,
                body.ExpireAt,
                body.BusinessId,
                body.Items),
            cancellationToken);

        return CreatedAtAction(
            nameof(ProposalsController.Get),
            "Proposals",
            new { proposalId },
            new { proposalId });
    }

    [HttpPut("{requestId:long}")]
    public async Task<IActionResult> Update(
        long requestId,
        [FromBody] CreateRequestCommand body,
        CancellationToken cancellationToken = default)
    {
        var updated = await _mediator.Send(new UpdateRequestCommand(requestId, body), cancellationToken);
        if (!updated)
            return NotFound();

        return Ok(new { requestId, success = true });
    }

    [HttpPost("{requestId:long}/cancel")]
    public async Task<IActionResult> Cancel(long requestId, CancellationToken cancellationToken = default)
    {
        var cancelled = await _mediator.Send(new CancelRequestCommand(requestId), cancellationToken);
        if (!cancelled)
            return NotFound();

        return Ok(new { requestId, status = "Cancelled" });
    }

    [HttpDelete("{requestId:long}")]
    public async Task<IActionResult> Delete(long requestId, CancellationToken cancellationToken = default)
    {
        var deleted = await _mediator.Send(new DeleteRequestCommand(requestId), cancellationToken);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

public sealed record CreateProposalBody(
    string ProposerType,
    decimal TotalPrice,
    decimal DeliveryFee = 0m,
    string? Message = null,
    DateOnly? ProposedDate = null,
    TimeSpan? ProposedTimeFrom = null,
    TimeSpan? ProposedTimeTo = null,
    DateTime? ExpireAt = null,
    long? BusinessId = null,
    IReadOnlyList<Matchi.Application.Features.Proposals.CreateProposalItemDto>? Items = null);

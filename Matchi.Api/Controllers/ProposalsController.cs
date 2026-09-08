using Matchi.Application.Features.Proposals.Commands.AcceptProposal;
using Matchi.Application.Features.Proposals.Commands.RejectProposal;
using Matchi.Application.Features.Proposals.Queries.GetProposalById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProposalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProposalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{proposalId:long}")]
    public async Task<IActionResult> Get(long proposalId, CancellationToken cancellationToken = default)
    {
        var proposal = await _mediator.Send(new GetProposalByIdQuery(proposalId), cancellationToken);
        if (proposal is null)
            return NotFound();

        return Ok(proposal);
    }

    [HttpPost("{proposalId:long}/accept")]
    public async Task<IActionResult> Accept(long proposalId, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new AcceptProposalCommand(proposalId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{proposalId:long}/reject")]
    public async Task<IActionResult> Reject(long proposalId, CancellationToken cancellationToken = default)
    {
        var status = await _mediator.Send(new RejectProposalCommand(proposalId), cancellationToken);
        return Ok(new { proposalId, status });
    }
}

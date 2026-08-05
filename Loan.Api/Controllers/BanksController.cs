using Loan.Application.Features.Banks.Queries.GetBanks;
using Loan.Application.Features.Banks.Queries.GetBankBySlug;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Loan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BanksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BanksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetBanks(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetBanksQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(
        string slug,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetBankBySlugQuery(slug),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
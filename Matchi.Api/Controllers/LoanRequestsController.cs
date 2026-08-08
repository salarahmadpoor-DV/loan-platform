using Matchi.Application.Features.LoanRequests.Commands.CreateLoanRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoanRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoanRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateLoanRequestCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(new
        {
            id
        });
    }
}
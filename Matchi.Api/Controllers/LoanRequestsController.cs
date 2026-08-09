using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.LoanRequests.Commands.CreateLoanRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoanRequestsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public LoanRequestsController(
        IMediator mediator,
        ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }


    public sealed record CreateLoanRequestDto(long BankId);

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromBody] CreateLoanRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var command = new CreateLoanRequestCommand
        {
            BankId = request.BankId,
            UserId = userId.Value
        };

        var id = await _mediator.Send(command, cancellationToken);

        return Ok(new
        {
            id
        });
    }
}
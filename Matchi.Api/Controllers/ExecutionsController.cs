using Matchi.Application.Features.Executions.Commands.CancelServiceExecution;
using Matchi.Application.Features.Executions.Commands.CompleteServiceExecution;
using Matchi.Application.Features.Executions.Commands.CreateExecutionAssignment;
using Matchi.Application.Features.Executions.Commands.RemoveExecutionAssignment;
using Matchi.Application.Features.Executions.Commands.StartServiceExecution;
using Matchi.Application.Features.Executions.Commands.UpdateServiceExecutionSchedule;
using Matchi.Application.Features.Executions.Queries.GetExecutionAssignments;
using Matchi.Application.Features.Executions.Queries.GetServiceExecutionById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExecutionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExecutionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{executionId:long}")]
    public async Task<IActionResult> Get(long executionId, CancellationToken cancellationToken = default)
    {
        var execution = await _mediator.Send(new GetServiceExecutionByIdQuery(executionId), cancellationToken);
        if (execution is null)
            return NotFound();

        return Ok(execution);
    }

    [HttpPut("{executionId:long}")]
    public async Task<IActionResult> UpdateSchedule(
        long executionId,
        [FromBody] UpdateExecutionScheduleBody body,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateServiceExecutionScheduleCommand(
                executionId,
                body.ScheduledDate,
                body.ScheduledTimeFrom,
                body.ScheduledTimeTo),
            cancellationToken);
        return Ok(new { executionId });
    }

    [HttpPost("{executionId:long}/start")]
    public async Task<IActionResult> Start(long executionId, CancellationToken cancellationToken = default)
    {
        var status = await _mediator.Send(new StartServiceExecutionCommand(executionId), cancellationToken);
        return Ok(new { executionId, status });
    }

    [HttpPost("{executionId:long}/complete")]
    public async Task<IActionResult> Complete(long executionId, CancellationToken cancellationToken = default)
    {
        var status = await _mediator.Send(new CompleteServiceExecutionCommand(executionId), cancellationToken);
        return Ok(new { executionId, status });
    }

    [HttpPost("{executionId:long}/cancel")]
    public async Task<IActionResult> Cancel(long executionId, CancellationToken cancellationToken = default)
    {
        var status = await _mediator.Send(new CancelServiceExecutionCommand(executionId), cancellationToken);
        return Ok(new { executionId, status });
    }

    [HttpGet("{executionId:long}/assignments")]
    public async Task<IActionResult> GetAssignments(long executionId, CancellationToken cancellationToken = default)
    {
        var assignments = await _mediator.Send(new GetExecutionAssignmentsQuery(executionId), cancellationToken);
        return Ok(assignments);
    }

    [HttpPost("{executionId:long}/assignments")]
    public async Task<IActionResult> CreateAssignment(
        long executionId,
        [FromBody] CreateExecutionAssignmentBody body,
        CancellationToken cancellationToken = default)
    {
        var assignmentId = await _mediator.Send(
            new CreateExecutionAssignmentCommand(executionId, body.ProviderId, body.Role, body.IsPrimary),
            cancellationToken);
        return CreatedAtAction(nameof(GetAssignments), new { executionId }, new { assignmentId });
    }

    [HttpDelete("{executionId:long}/assignments/{assignmentId:long}")]
    public async Task<IActionResult> RemoveAssignment(
        long executionId,
        long assignmentId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new RemoveExecutionAssignmentCommand(executionId, assignmentId), cancellationToken);
        return NoContent();
    }
}

public sealed record UpdateExecutionScheduleBody(
    DateOnly? ScheduledDate = null,
    TimeSpan? ScheduledTimeFrom = null,
    TimeSpan? ScheduledTimeTo = null);

public sealed record CreateExecutionAssignmentBody(
    long ProviderId,
    string Role,
    bool? IsPrimary = null);

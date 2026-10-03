using Matchi.Application.Features.Notifications.Commands;
using Matchi.Application.Features.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMine(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetMyNotificationsQuery(page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUnreadNotificationCountQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{notificationId:long}/read")]
    public async Task<IActionResult> MarkRead(long notificationId, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new MarkNotificationAsReadCommand(notificationId), cancellationToken);
        return Ok();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken = default)
    {
        var count = await _mediator.Send(new MarkAllNotificationsAsReadCommand(), cancellationToken);
        return Ok(new { updated = count });
    }
}

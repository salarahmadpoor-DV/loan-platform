using Matchi.Application.Features.Businesses;
using Matchi.Application.Features.Businesses.Commands;
using Matchi.Application.Features.Businesses.Queries;
using Matchi.Application.Features.Businesses.Queries.GetBusinesses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusinessesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusinessesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "BUSINESS_CREATE")]
    public async Task<IActionResult> Create(
        [FromBody] CreateBusinessCommand command,
        CancellationToken cancellationToken = default)
    {
        var businessId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(List), new { businessId }, new { businessId });
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var businesses = await _mediator.Send(new GetBusinessesQuery(page, pageSize), cancellationToken);
        return Ok(new { page, pageSize, items = businesses });
    }

    [HttpGet("me")]
    [Authorize(Policy = "BUSINESS_VIEW")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMyBusinessesQuery(), cancellationToken));
    }

    [HttpPut("me")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> UpdateMine(
        [FromBody] UpdateMyBusinessCommand command,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(command, cancellationToken);
        return Ok(new { success = true });
    }

    [HttpGet("me/services")]
    [Authorize(Policy = "BUSINESS_VIEW")]
    public async Task<IActionResult> GetMyServices(
        [FromQuery] long? businessId,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMyBusinessServicesQuery(businessId), cancellationToken));
    }

    [HttpPost("me/services")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> AddMyService(
        [FromBody] AddBusinessServiceCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { id, command.ServiceId });
    }

    [HttpPut("me/services/{serviceId:long}")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> UpdateMyService(
        long serviceId,
        [FromBody] UpdateBusinessServiceCommand body,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(body with { ServiceId = serviceId }, cancellationToken);
        return Ok(new { serviceId, success = true });
    }

    [HttpDelete("me/services/{serviceId:long}")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> DeleteMyService(
        long serviceId,
        [FromQuery] long? businessId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteBusinessServiceCommand(businessId, serviceId), cancellationToken);
        return NoContent();
    }

    [HttpGet("me/products")]
    [Authorize(Policy = "BUSINESS_VIEW")]
    public async Task<IActionResult> GetMyProducts(
        [FromQuery] long? businessId,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMyBusinessProductsQuery(businessId), cancellationToken));
    }

    [HttpPost("me/products")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> AddMyProduct(
        [FromBody] AddBusinessProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { id, command.ProductId });
    }

    [HttpPut("me/products/{productId:long}")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> UpdateMyProduct(
        long productId,
        [FromBody] UpdateBusinessProductCommand body,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(body with { ProductId = productId }, cancellationToken);
        return Ok(new { productId, success = true });
    }

    [HttpDelete("me/products/{productId:long}")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> DeleteMyProduct(
        long productId,
        [FromQuery] long? businessId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteBusinessProductCommand(businessId, productId), cancellationToken);
        return NoContent();
    }

    [HttpGet("me/areas")]
    [Authorize(Policy = "BUSINESS_VIEW")]
    public async Task<IActionResult> GetMyAreas(
        [FromQuery] long? businessId,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMyBusinessServiceAreasQuery(businessId), cancellationToken));
    }

    [HttpPost("me/areas")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> AddMyArea(
        [FromBody] AddBusinessServiceAreaCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { id });
    }

    [HttpPut("me/areas/{areaId:long}")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> UpdateMyArea(
        long areaId,
        [FromBody] UpdateBusinessServiceAreaCommand body,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(body with { AreaId = areaId }, cancellationToken);
        return Ok(new { areaId, success = true });
    }

    [HttpDelete("me/areas/{areaId:long}")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> DeleteMyArea(
        long areaId,
        [FromQuery] long? businessId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteBusinessServiceAreaCommand(businessId, areaId), cancellationToken);
        return NoContent();
    }

    [HttpGet("me/availabilities")]
    [Authorize(Policy = "BUSINESS_VIEW")]
    public async Task<IActionResult> GetMyAvailabilities(
        [FromQuery] long? businessId,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMyBusinessAvailabilitiesQuery(businessId), cancellationToken));
    }

    [HttpPost("me/availabilities")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> AddMyAvailability(
        [FromBody] AddBusinessAvailabilityCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { id });
    }

    [HttpPut("me/availabilities/{availabilityId:long}")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> UpdateMyAvailability(
        long availabilityId,
        [FromBody] UpdateBusinessAvailabilityCommand body,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(body with { AvailabilityId = availabilityId }, cancellationToken);
        return Ok(new { availabilityId, success = true });
    }

    [HttpDelete("me/availabilities/{availabilityId:long}")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> DeleteMyAvailability(
        long availabilityId,
        [FromQuery] long? businessId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteBusinessAvailabilityCommand(businessId, availabilityId), cancellationToken);
        return NoContent();
    }

    [HttpGet("me/providers")]
    [Authorize(Policy = "BUSINESS_VIEW")]
    public async Task<IActionResult> GetMyProviders(
        [FromQuery] long? businessId,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMyBusinessProvidersQuery(businessId), cancellationToken));
    }

    [HttpPost("me/providers")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> AddMyProvider(
        [FromBody] AddBusinessProviderCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { id, command.ProviderId });
    }

    [HttpPut("me/providers/{providerId:long}")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> UpdateMyProvider(
        long providerId,
        [FromBody] UpdateBusinessProviderCommand body,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(body with { ProviderId = providerId }, cancellationToken);
        return Ok(new { providerId, success = true });
    }

    [HttpDelete("me/providers/{providerId:long}")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> DeleteMyProvider(
        long providerId,
        [FromQuery] long? businessId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteBusinessProviderCommand(businessId, providerId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{businessId:long}/invite-provider")]
    [Authorize(Policy = "BUSINESS_EDIT")]
    public async Task<IActionResult> InviteProvider(
        long businessId,
        [FromBody] InviteDto dto,
        CancellationToken cancellationToken = default)
    {
        var inviteId = await _mediator.Send(
            new InviteProviderCommand(businessId, dto.ProviderId, dto.Role),
            cancellationToken);
        return Accepted(new { inviteId, status = "Pending" });
    }

    [HttpPut("business-providers/{businessProviderId:long}/accept")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> AcceptMembership(
        long businessProviderId,
        CancellationToken cancellationToken = default)
    {
        var success = await _mediator.Send(
            new AcceptBusinessMembershipCommand(businessProviderId),
            cancellationToken);
        if (!success)
            return NotFound();

        return Ok(new { businessProviderId, status = "Active" });
    }

    public sealed record InviteDto(long ProviderId, string? Role);
}

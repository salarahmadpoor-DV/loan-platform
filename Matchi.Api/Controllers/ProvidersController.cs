using Matchi.Application.Common;
using Matchi.Application.Features.Providers;
using Matchi.Application.Features.Providers.Commands;
using Matchi.Application.Features.Providers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvidersController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProvidersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "PROVIDER_CREATE")]
    public async Task<IActionResult> Create(
        [FromBody] CreateProviderCommand command,
        CancellationToken cancellationToken = default)
    {
        var providerId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Get), new { providerId }, new { providerId });
    }

    [HttpGet("me")]
    [Authorize(Policy = "PROVIDER_VIEW")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken = default)
    {
        var provider = await _mediator.Send(new GetMyProviderQuery(), cancellationToken);
        if (provider is null)
            return NotFound();

        return Ok(provider);
    }

    [HttpPut("me")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> UpdateMine(
        [FromBody] UpdateMyProviderCommand command,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(command, cancellationToken);
        return Ok(new { success = true });
    }

    [HttpGet("me/services")]
    [Authorize(Policy = "PROVIDER_VIEW")]
    public async Task<IActionResult> GetMyServices(CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMyProviderServicesQuery(), cancellationToken));
    }

    [HttpPost("me/services")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> AddMyService(
        [FromBody] AddProviderServiceCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { id, command.ServiceId });
    }

    [HttpPut("me/services/{serviceId:long}")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> UpdateMyService(
        long serviceId,
        [FromBody] UpdateProviderServiceBody body,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new UpdateProviderServiceCommand(serviceId, body.IsActive), cancellationToken);
        return Ok(new { serviceId, success = true });
    }

    [HttpDelete("me/services/{serviceId:long}")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> DeleteMyService(long serviceId, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteProviderServiceCommand(serviceId), cancellationToken);
        return NoContent();
    }

    [HttpGet("me/products")]
    [Authorize(Policy = "PROVIDER_VIEW")]
    public async Task<IActionResult> GetMyProducts(CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMyProviderProductsQuery(), cancellationToken));
    }

    [HttpPost("me/products")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> AddMyProduct(
        [FromBody] AddProviderProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { id, command.ProductId });
    }

    [HttpPut("me/products/{productId:long}")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> UpdateMyProduct(
        long productId,
        [FromBody] UpdateProviderProductBody body,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateProviderProductCommand(
                productId, body.Price, body.IsAvailable, body.MinOrderQuantity, body.LeadTimeDays),
            cancellationToken);
        return Ok(new { productId, success = true });
    }

    [HttpDelete("me/products/{productId:long}")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> DeleteMyProduct(long productId, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteProviderProductCommand(productId), cancellationToken);
        return NoContent();
    }

    [HttpGet("me/capabilities")]
    [Authorize(Policy = "PROVIDER_VIEW")]
    public async Task<IActionResult> GetMyCapabilities(CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMyProviderCapabilitiesQuery(), cancellationToken));
    }

    [HttpPost("me/capabilities")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> AddMyCapability(
        [FromBody] AddProviderCapabilityCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { id, command.ServiceAttributeId });
    }

    [HttpPut("me/capabilities/{serviceAttributeId:long}")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> UpdateMyCapability(
        long serviceAttributeId,
        [FromBody] UpdateProviderCapabilityBody body,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new UpdateProviderCapabilityCommand(serviceAttributeId, body.Value), cancellationToken);
        return Ok(new { serviceAttributeId, success = true });
    }

    [HttpDelete("me/capabilities/{serviceAttributeId:long}")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> DeleteMyCapability(
        long serviceAttributeId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteProviderCapabilityCommand(serviceAttributeId), cancellationToken);
        return NoContent();
    }

    [HttpGet("me/areas")]
    [Authorize(Policy = "PROVIDER_VIEW")]
    public async Task<IActionResult> GetMyAreas(CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMyProviderServiceAreasQuery(), cancellationToken));
    }

    [HttpPost("me/areas")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> AddMyArea(
        [FromBody] AddProviderServiceAreaCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { id });
    }

    [HttpPut("me/areas/{areaId:long}")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> UpdateMyArea(
        long areaId,
        [FromBody] AddProviderServiceAreaCommand body,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateProviderServiceAreaCommand(
                areaId, body.AreaType, body.Province, body.City, body.District, body.Lat, body.Lng, body.Radius, body.IsActive),
            cancellationToken);
        return Ok(new { areaId, success = true });
    }

    [HttpDelete("me/areas/{areaId:long}")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> DeleteMyArea(long areaId, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteProviderServiceAreaCommand(areaId), cancellationToken);
        return NoContent();
    }

    [HttpGet("me/availabilities")]
    [Authorize(Policy = "PROVIDER_VIEW")]
    public async Task<IActionResult> GetMyAvailabilities(CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMyProviderAvailabilitiesQuery(), cancellationToken));
    }

    [HttpPost("me/availabilities")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> AddMyAvailability(
        [FromBody] AddProviderAvailabilityCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { id });
    }

    [HttpPut("me/availabilities/{availabilityId:long}")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> UpdateMyAvailability(
        long availabilityId,
        [FromBody] AddProviderAvailabilityCommand body,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateProviderAvailabilityCommand(
                availabilityId, body.DayOfWeek, body.TimeFrom, body.TimeTo, body.IsAvailable),
            cancellationToken);
        return Ok(new { availabilityId, success = true });
    }

    [HttpDelete("me/availabilities/{availabilityId:long}")]
    [Authorize(Policy = "PROVIDER_EDIT")]
    public async Task<IActionResult> DeleteMyAvailability(
        long availabilityId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteProviderAvailabilityCommand(availabilityId), cancellationToken);
        return NoContent();
    }

    [HttpGet("me/businesses")]
    [Authorize(Policy = "PROVIDER_VIEW")]
    public async Task<IActionResult> GetMyBusinesses(CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMyProviderBusinessesQuery(), cancellationToken));
    }

    [HttpGet("{providerId:long}")]
    public async Task<IActionResult> Get(long providerId, CancellationToken cancellationToken = default)
    {
        var provider = await _mediator.Send(new GetProviderByIdQuery(providerId), cancellationToken);
        if (provider is null)
            return NotFound();

        return Ok(provider);
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] long? serviceId,
        [FromQuery] double? lat = null,
        [FromQuery] double? lng = null,
        [FromQuery] int radiusKm = 10,
        [FromQuery] string? sort = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var paging = ListPaging.Normalize(page, pageSize);
        var providers = await _mediator.Send(
            new SearchProvidersQuery(serviceId, lat, lng, radiusKm, sort, paging.Page, paging.PageSize),
            cancellationToken);
        return Ok(new { page = paging.Page, pageSize = paging.PageSize, items = providers });
    }

    public sealed record UpdateProviderServiceBody(bool IsActive);

    public sealed record UpdateProviderProductBody(
        decimal? Price,
        bool IsAvailable,
        decimal? MinOrderQuantity,
        int? LeadTimeDays);

    public sealed record UpdateProviderCapabilityBody(string Value);
}

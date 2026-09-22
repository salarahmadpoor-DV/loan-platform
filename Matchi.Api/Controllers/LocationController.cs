using Matchi.Application.Features.Locations.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api/locations")]
public sealed class LocationController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("provinces")]
    public async Task<IActionResult> GetProvinces(CancellationToken cancellationToken = default)
    {
        var provinces = await _mediator.Send(new GetActiveProvincesQuery(), cancellationToken);
        return Ok(provinces);
    }

    [HttpGet("provinces/{provinceId:long}/cities")]
    public async Task<IActionResult> GetCitiesByProvince(
        long provinceId,
        CancellationToken cancellationToken = default)
    {
        var cities = await _mediator.Send(new GetCitiesByProvinceQuery(provinceId), cancellationToken);
        return Ok(cities);
    }

    [HttpGet("cities/{cityId:long}/districts")]
    public async Task<IActionResult> GetDistrictsByCity(
        long cityId,
        CancellationToken cancellationToken = default)
    {
        var districts = await _mediator.Send(new GetDistrictsByCityQuery(cityId), cancellationToken);
        return Ok(districts);
    }

    [HttpGet("resolve")]
    public async Task<IActionResult> Resolve(
        [FromQuery] decimal lat,
        [FromQuery] decimal lng,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ResolveLocationQuery(lat, lng), cancellationToken);
        return Ok(result);
    }
}

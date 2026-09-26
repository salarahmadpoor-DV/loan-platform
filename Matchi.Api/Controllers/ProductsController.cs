using Matchi.Application.Common;
using Matchi.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Matchi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken = default)
    {
        var categories = await _mediator.Send(new GetProductCategoriesQuery(), cancellationToken);
        return Ok(categories);
    }

    [HttpGet("categories/{categoryId:long}/attributes")]
    public async Task<IActionResult> GetCategoryAttributes(
        long categoryId,
        CancellationToken cancellationToken = default)
    {
        var attributes = await _mediator.Send(
            new GetProductCategoryAttributesQuery(categoryId),
            cancellationToken);
        return Ok(attributes);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] long? categoryId,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var paging = ListPaging.Normalize(page, pageSize);
        var products = await _mediator.Send(
            new GetProductsQuery(categoryId, q, paging.Page, paging.PageSize),
            cancellationToken);
        return Ok(new { page = paging.Page, pageSize = paging.PageSize, items = products });
    }

    [HttpGet("{productId:long}")]
    public async Task<IActionResult> GetProduct(long productId, CancellationToken cancellationToken = default)
    {
        var item = await _mediator.Send(new GetProductByIdQuery(productId), cancellationToken);
        if (item is null)
            return NotFound();
        return Ok(item);
    }
}

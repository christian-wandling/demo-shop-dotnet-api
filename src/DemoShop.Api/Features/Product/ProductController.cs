using Asp.Versioning;
using AutoMapper;
using DemoShop.Api.Common;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.Product.Queries.GetAllProducts;
using DemoShop.Application.Features.Product.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoShop.Api.Features.Product;

[ApiVersion("1.0")]
[AllowAnonymous]
[Route("api/v{version:apiVersion}/products")]
public class ProductController(IMediator mediator) : ApiController
{
    [HttpGet("")]
    [ProducesResponseType(typeof(ProductListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductListResponse>> GetAllProducts(CancellationToken cancellationToken)
    {
        var query = new GetAllProductsQuery();
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse?>> GetProductById(
        int id,
        CancellationToken cancellationToken
    )
    {
        var query = new GetProductByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return ToActionResult(result);
    }
}

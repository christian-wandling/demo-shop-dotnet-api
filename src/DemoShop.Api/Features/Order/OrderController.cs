using Asp.Versioning;
using DemoShop.Api.Common;
using DemoShop.Application.Features.Order.Commands.CreateOrder;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Order.Queries.GetAllOrdersOfUser;
using DemoShop.Application.Features.Order.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoShop.Api.Features.Order;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
[Route("api/v{version:apiVersion}/orders")]
public class OrderController(IMediator mediator) : ApiController
{
    [HttpGet("")]
    [ProducesResponseType(typeof(OrderListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderListResponse>> GetAllOrders(CancellationToken cancellationToken)
    {
        var query = new GetAllOrdersOfUserQuery();
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse?>> GetOrderById(
        int id,
        CancellationToken cancellationToken
    )
    {
        var query = new GetOrderByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return ToActionResult(result);
    }

    [HttpPost("")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse?>> CreateOrderFromShoppingSession(CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand();
        var result = await mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return ToActionResult(result);
    }
}

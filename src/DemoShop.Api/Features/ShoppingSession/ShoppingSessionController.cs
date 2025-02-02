using Asp.Versioning;
using DemoShop.Api.Common;
using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Features.ShoppingSession.Commands.AddCartItem;
using DemoShop.Application.Features.ShoppingSession.Commands.RemoveCartItem;
using DemoShop.Application.Features.ShoppingSession.Commands.UpdateCartItemQuantity;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Queries.GetOrCreateShoppingSession;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoShop.Api.Features.ShoppingSession;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
[Route("api/v{version:apiVersion}/shopping-sessions/current")]
public sealed class ShoppingSessionController(IMediator mediator) : ApiController
{
    [HttpGet("")]
    [ProducesResponseType(typeof(ShoppingSessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ShoppingSessionResponse?>> GetCurrentShoppingSession(
        CancellationToken cancellationToken)
    {
        var query = new GetOrCreateShoppingSessionQuery();
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return ToActionResult(result);
    }

    [HttpPost("cart-items")]
    [ProducesResponseType(typeof(CartItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CartItemResponse>> AddCartItem(
        [FromBody] AddCartItemRequest addCartItem,
        CancellationToken cancellationToken
    )
    {
        var query = new AddCartItemCommand(addCartItem);
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return ToActionResult(result);
    }

    [HttpPatch("cart-items/{id:int}")]
    [ProducesResponseType(typeof(UpdateCartItemQuantityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UpdateCartItemQuantityResponse>> UpdateCartItemQuantity(
        int id,
        [FromBody] UpdateCartItemRequest updateCartItem,
        CancellationToken cancellationToken
    )
    {
        var query = new UpdateCartItemQuantityCommand(id, updateCartItem);
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return ToActionResult(result);
    }

    [HttpDelete("cart-items/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RemoveCartItem(int id, CancellationToken cancellationToken)
    {
        var query = new RemoveCartItemCommand(id);
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return ToActionResult(result);
    }
}

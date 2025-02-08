#region

using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Features.ShoppingSession.Commands.AddCartItem;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.ShoppingSession.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class AddCartItemEndpoint(IMediator mediator)
    : EndpointBaseAsync.WithRequest<AddCartItemRequest>.WithResult<Result<CartItemResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.Error, ResultStatus.NotFound)]
    [HttpPost("api/v{version:apiVersion}/shopping-sessions/current/cart-items")]
    [SwaggerOperation(
        Summary = "Add cart item",
        Description =
            "Add a cart item to the shopping session of current user based on identity extracted from bearer token",
        OperationId = "ShoppingSession.AddCartItem",
        Tags = ["ShoppingSession"])
    ]
    public override async Task<Result<CartItemResponse>> HandleAsync(
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken = default
    ) =>
        await mediator.Send(new AddCartItemCommand(request), cancellationToken);
}

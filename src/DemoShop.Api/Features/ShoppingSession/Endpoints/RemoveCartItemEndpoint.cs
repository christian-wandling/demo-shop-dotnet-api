#region

using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Features.ShoppingSession.Commands.RemoveCartItem;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.ShoppingSession.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class RemoveCartItemEndpoint(IMediator mediator)
    : EndpointBaseAsync.WithRequest<RemoveCartItemRequest>.WithResult<Result>
{
    [TranslateResultToActionResult]
    [HttpDelete("api/v{version:apiVersion}/shopping-sessions/current/cart-items/{id:int}")]
    [ExpectedFailures(
        ResultStatus.Unauthorized,
        ResultStatus.Forbidden,
        ResultStatus.Error,
        ResultStatus.NotFound
    )]
    [SwaggerOperation(
        Summary = "Remove cart item",
        Description =
            "Remove a cart item from the shopping session of current user based on identity extracted from bearer token",
        OperationId = "RemoveCartItem",
        Tags = ["ShoppingSession"])
    ]
    public override async Task<Result> HandleAsync(
        [FromRoute] RemoveCartItemRequest request,
        CancellationToken cancellationToken = default
    )
    {
        Guard.Against.Null(request, nameof(request));
        return await mediator.Send(new RemoveCartItemCommand(request.Id), cancellationToken);
    }
}

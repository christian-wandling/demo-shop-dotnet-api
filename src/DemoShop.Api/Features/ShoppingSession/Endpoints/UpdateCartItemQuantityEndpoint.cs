#region

using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Features.ShoppingSession.Commands.UpdateCartItemQuantity;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.ShoppingSession.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class UpdateCartItemQuantityEndpoint(IMediator mediator)
    : EndpointBaseAsync.WithRequest<UpdateCartItemQuantityRequestWrapper>.WithResult<
        Result<UpdateCartItemQuantityResponse>>
{
    [TranslateResultToActionResult]
    [HttpPatch("api/v{version:apiVersion}/shopping-sessions/current/cart-items/{id:int}")]
    [ExpectedFailures(
        ResultStatus.Unauthorized,
        ResultStatus.Forbidden,
        ResultStatus.Error,
        ResultStatus.NotFound,
        ResultStatus.Invalid
    )]
    [SwaggerOperation(
        Summary = "Update cart item quantity",
        Description =
            "Update quantity of a cart item in shopping session of current user based on identity extracted from bearer token",
        OperationId = "UpdateCartItemQuantity",
        Tags = ["ShoppingSession"])
    ]
    public override async Task<Result<UpdateCartItemQuantityResponse>> HandleAsync(
        [FromRoute] UpdateCartItemQuantityRequestWrapper request,
        CancellationToken cancellationToken = default
    )
    {
        Guard.Against.Null(request, nameof(request));

        return await mediator.Send(new UpdateCartItemQuantityCommand(request.Id, request.UpdateCartItemQuantityRequest),
            cancellationToken);
    }
}

#region

using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.ShoppingSession.Processes.Checkout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.ShoppingSession.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class CheckoutEndpoint(IMediator mediator)
    : EndpointBaseAsync.WithoutRequest.WithResult<Result<OrderResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.Error)]
    [HttpPost("api/v{version:apiVersion}/shopping-sessions/checkout")]
    [SwaggerOperation(
        Summary = "Checkout the shopping session",
        Description =
            "Check out by creating an order from the current shopping session and deleting the shopping session afterwardds",
        OperationId = "Checkout",
        Tags = ["ShoppingSession"])
    ]
    public override async Task<Result<OrderResponse>> HandleAsync(CancellationToken cancellationToken = default) =>
        await mediator.Send(new CheckoutProcess(), cancellationToken);
}

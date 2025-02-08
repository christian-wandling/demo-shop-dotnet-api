#region

using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Application.Features.Order.Commands.CreateOrder;
using DemoShop.Application.Features.Order.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.Order.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class CreateOrderEndpoint(IMediator mediator)
    : EndpointBaseAsync.WithoutRequest.WithResult<Result<OrderResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.Error)]
    [HttpPost("api/v{version:apiVersion}/orders")]
    [SwaggerOperation(
        Summary = "Create order",
        Description =
            "Create order from shopping session of current user based on identity extracted from bearer token",
        OperationId = "Order.CreateOrder",
        Tags = ["Order"])
    ]
    public override async Task<Result<OrderResponse>> HandleAsync(CancellationToken cancellationToken = default) =>
        await mediator.Send(new CreateOrderCommand(), cancellationToken);
}

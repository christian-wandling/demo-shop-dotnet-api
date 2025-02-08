#region

using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Order.Queries.GetAllOrdersOfUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.Order.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class GetAllOrdersOfUserEndpoint(IMediator mediator)
    : EndpointBaseAsync.WithoutRequest.WithResult<Result<OrderListResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden)]
    [HttpGet("api/v{version:apiVersion}/orders")]
    [SwaggerOperation(
        Summary = "Get all orders",
        Description = "Get all orders of current user based on identity extracted from bearer token",
        OperationId = "Order.GetAllOrders",
        Tags = ["Order"])
    ]
    public override async Task<Result<OrderListResponse>> HandleAsync(CancellationToken cancellationToken = default) =>
        await mediator.Send(new GetAllOrdersOfUserQuery(), cancellationToken);
}

#region

using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.Order.Models;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Order.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.Order.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class GetOrderByIdEndpoint(IMediator mediator)
    : EndpointBaseAsync.WithRequest<GetOrderByIdRequest>.WithResult<Result<OrderResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.NotFound, ResultStatus.Error)]
    [HttpGet("api/v{version:apiVersion}/orders/{id:int}")]
    [SwaggerOperation(
        Summary = "Get order by id",
        Description = "Get order by id of current user based on identity extracted from bearer token",
        OperationId = "GetOrderById",
        Tags = ["Order"])
    ]
    public override async Task<Result<OrderResponse>> HandleAsync(
        [FromRoute] GetOrderByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(request, nameof(request));
        return await mediator.Send(new GetOrderByIdQuery(request.Id), cancellationToken);
    }
}

#region

using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Queries.GetOrCreateShoppingSession;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.ShoppingSession.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class GetCurrentShoppingSessionEndpoint(IMediator mediator)
    : EndpointBaseAsync.WithoutRequest.WithResult<Result<ShoppingSessionResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.Error, ResultStatus.NotFound)]
    [HttpGet("api/v{version:apiVersion}/shopping-sessions/current")]
    [SwaggerOperation(
        Summary = "Get current shopping session",
        Description = "Get the shopping session of current user based on identity extracted from bearer token",
        OperationId = "ShoppingSession.GetCurrentShoppingSession",
        Tags = ["ShoppingSession"])
    ]
    public override async Task<Result<ShoppingSessionResponse>> HandleAsync(
        CancellationToken cancellationToken = default) =>
        await mediator.Send(new GetOrCreateShoppingSessionQuery(), cancellationToken);
}

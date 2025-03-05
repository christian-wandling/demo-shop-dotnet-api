#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Features.ShoppingSession.Commands.AddCartItem;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Domain.Common.Logging;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ILogger = Serilog.ILogger;

#endregion

namespace DemoShop.Api.Features.ShoppingSession.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class AddCartItemEndpoint(IMediator mediator, ILogger logger)
    : EndpointBaseAsync.WithRequest<AddCartItemRequest>.WithResult<Result<CartItemResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.Error, ResultStatus.NotFound)]
    [HttpPost("api/v{version:apiVersion}/shopping-sessions/current/cart-items")]
    [SwaggerOperation(
        Summary = "Add cart item",
        Description =
            "Add a cart item to the shopping session of current user based on identity extracted from bearer token",
        OperationId = "AddCartItem",
        Tags = ["ShoppingSession"])
    ]
    public override async Task<Result<CartItemResponse>> HandleAsync(
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, "Add cart item to current shopping session");

        var result = await mediator.Send(new AddCartItemCommand(request), cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, result.Value.Id, stopwatch.Elapsed.Milliseconds);
        else
            LogRequestFailed(logger, stopwatch.Elapsed.Milliseconds);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventId.AddCartItemRequestStarted)
            .Debug("Starting POST request for adding cart item to current shopping session at {Endpoint}",
                endpoint);

    private static void LogRequestSuccess(ILogger logger, int id, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.AddCartItemRequestSuccess)
            .Information(
                "Completed POST request for adding cart item to current shopping session {Id} in {ElapsedMs}ms",
                id, elapsedMs);

    private static void LogRequestFailed(ILogger logger, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.AddCartItemRequestFailed)
            .Error("Failed POST request to adding cart item to current shopping session in {ElapsedMs}ms", elapsedMs);
}

#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Features.ShoppingSession.Commands.RemoveCartItem;
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
public class RemoveCartItemEndpoint(IMediator mediator, ILogger logger)
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
        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, "Remove cart item from current shopping session");

        var result = await mediator.Send(new RemoveCartItemCommand(request.Id), cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, request.Id, stopwatch.Elapsed);
        else
            LogRequestFailed(logger, stopwatch.Elapsed);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventIds.RemoveCartItemRequestStarted)
            .Information("Starting POST request for removing cart item from current shopping session at {Endpoint}",
                endpoint);

    private static void LogRequestSuccess(ILogger logger, int id, TimeSpan elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventIds.RemoveCartItemRequestSuccess)
            .Information(
                "Completed POST request for removing cart item from current shopping session {Id} in {ElapsedMs}ms",
                id, elapsedMs);

    private static void LogRequestFailed(ILogger logger, TimeSpan elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventIds.RemoveCartItemRequestFailed)
            .Error("Failed POST request to removing cart item from current shopping session in {ElapsedMs}ms",
                elapsedMs);
}

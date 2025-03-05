#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Processes.ResolveShoppingSession;
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
public class ResolveCurrentShoppingSessionEndpoint(IMediator mediator, ILogger logger)
    : EndpointBaseAsync.WithoutRequest.WithResult<Result<ShoppingSessionResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.Error, ResultStatus.NotFound)]
    [HttpPost("api/v{version:apiVersion}/shopping-sessions/current")]
    [SwaggerOperation(
        Summary = "Resolve current shopping session",
        Description = "Resolve current shopping session based on identity extracted from bearer token",
        OperationId = "ResolveCurrentShoppingSession",
        Tags = ["ShoppingSession"])
    ]
    public override async Task<Result<ShoppingSessionResponse>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, "Resolve current shopping session");

        var result = await mediator.Send(new ResolveShoppingSessionProcess(), cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, result.Value.Id, stopwatch.Elapsed.Milliseconds);
        else
            LogRequestFailed(logger, stopwatch.Elapsed.Milliseconds);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventId.ResolveCurrentShoppingSessionRequestStarted)
            .Debug("Starting POST request for resolving current shopping session at {Endpoint}", endpoint);

    private static void LogRequestSuccess(ILogger logger, int id, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.ResolveCurrentShoppingSessionRequestSuccess)
            .Information(
                "Completed POST request for resolving current shopping session {Id} in {ElapsedMs}ms",
                id, elapsedMs);

    private static void LogRequestFailed(ILogger logger, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.ResolveCurrentShoppingSessionRequestFailed)
            .Error("Failed POST request to resolving current shopping session in {ElapsedMs}ms", elapsedMs);
}

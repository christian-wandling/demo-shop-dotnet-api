#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.ShoppingSession.Processes.Checkout;
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
public class CheckoutEndpoint(IMediator mediator, ILogger logger)
    : EndpointBaseAsync.WithoutRequest.WithResult<Result<OrderResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.Error)]
    [HttpPost("api/v{version:apiVersion}/shopping-sessions/checkout")]
    [SwaggerOperation(
        Summary = "Checkout current shopping session",
        Description =
            "Check out by creating an order from the current shopping session and deleting the shopping session afterwards",
        OperationId = "Checkout",
        Tags = ["ShoppingSession"])
    ]
    public override async Task<Result<OrderResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, "Checkout current shopping session");

        var result = await mediator.Send(new CheckoutProcess(), cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, result.Value.Id, stopwatch.Elapsed.Milliseconds);
        else
            LogRequestFailed(logger, stopwatch.Elapsed.Milliseconds);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventId.CheckoutRequestStarted)
            .Debug("Starting POST request for checkout current shopping session at {Endpoint}", endpoint);

    private static void LogRequestSuccess(ILogger logger, int id, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.CheckoutRequestSuccess)
            .Information(
                "Completed POST request for checkout current shopping session {Id} in {ElapsedMs}ms",
                id, elapsedMs);

    private static void LogRequestFailed(ILogger logger, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.CheckoutRequestFailed)
            .Error("Failed POST request to checkout current shopping session in {ElapsedMs}ms", elapsedMs);
}

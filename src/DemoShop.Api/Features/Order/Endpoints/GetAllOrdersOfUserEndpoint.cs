#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Order.Queries.GetAllOrdersOfUser;
using DemoShop.Domain.Common.Logging;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ILogger = Serilog.ILogger;

#endregion

namespace DemoShop.Api.Features.Order.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class GetAllOrdersOfUserEndpoint(IMediator mediator, ILogger logger)
    : EndpointBaseAsync.WithoutRequest.WithResult<Result<OrderListResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden)]
    [HttpGet("api/v{version:apiVersion}/orders")]
    [SwaggerOperation(
        Summary = "Get all orders",
        Description = "Get all orders of current user based on identity extracted from bearer token",
        OperationId = "GetAllOrdersOfCurrentUser",
        Tags = ["Order"])
    ]
    public override async Task<Result<OrderListResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, "Get orders of current user");

        var result = await mediator.Send(new GetAllOrdersOfUserQuery(), cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, stopwatch.Elapsed.Milliseconds);
        else
            LogRequestFailed(logger, stopwatch.Elapsed.Milliseconds);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventId.GetAllOrdersOfUserRequestStarted)
            .Debug("Starting GET request for all orders of current user at {Endpoint}", endpoint);

    private static void LogRequestSuccess(ILogger logger, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.GetAllOrdersOfUserRequestSuccess)
            .Information(
                "Completed GET request for all orders of current user in {ElapsedMs}ms", elapsedMs);

    private static void LogRequestFailed(ILogger logger, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.GetAllOrdersOfUserRequestFailed)
            .Warning("Failed to retrieve orders of current user in {ElapsedMs}ms", elapsedMs);
}

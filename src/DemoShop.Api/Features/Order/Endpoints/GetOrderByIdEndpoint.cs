#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.Order.Models;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Order.Queries.GetOrderById;
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
public class GetOrderByIdEndpoint(IMediator mediator, ILogger logger)
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

        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, request.Id, "Get order by id");

        var result = await mediator.Send(new GetOrderByIdQuery(request.Id), cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, request.Id, stopwatch.Elapsed.Milliseconds);
        else
            LogRequestFailed(logger, request.Id, stopwatch.Elapsed.Milliseconds);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, int id, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventId.GetOrderByIdRequestStarted)
            .Debug("Starting GET request for order with id {Id} at {Endpoint}", id, endpoint);

    private static void LogRequestSuccess(ILogger logger, int id, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.GetOrderByIdRequestSuccess)
            .Information(
                "Completed GET request for order {Id} in {ElapsedMs}ms",
                id, elapsedMs);

    private static void LogRequestFailed(ILogger logger, int id, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.GetOrderByIdRequestFailed)
            .Warning("Failed to retrieve order with id {Id} in {ElapsedMs}ms", id, elapsedMs);
}

#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.Product.Models;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.Product.Queries.GetProductById;
using DemoShop.Domain.Common.Logging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ILogger = Serilog.ILogger;

#endregion

namespace DemoShop.Api.Features.Product.Endpoints;

[ApiVersion("1.0")]
public class GetProductByIdEndpoint(IMediator mediator, ILogger logger)
    : EndpointBaseAsync.WithRequest<GetProductByIdRequest>.WithResult<Result<ProductResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.NotFound)]
    [HttpGet("api/v{version:apiVersion}/products/{id:int}")]
    [SwaggerOperation(
        Summary = "Get all products",
        Description = "Get all products",
        OperationId = "GetProductById",
        Tags = ["Product"])
    ]
    public override async Task<Result<ProductResponse>> HandleAsync(
        [FromRoute] GetProductByIdRequest request,
        CancellationToken cancellationToken = default
    )
    {
        Guard.Against.Null(request, nameof(request));

        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, request.Id, "Get product by id");

        var result = await mediator.Send(new GetProductByIdQuery(request.Id), cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, request.Id, stopwatch.Elapsed);
        else
            LogRequestFailed(logger, request.Id, stopwatch.Elapsed);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, int id, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetProductByIdRequestStarted)
            .Information("Starting GET request for product with id {Id} at {Endpoint}", id, endpoint);

    private static void LogRequestSuccess(ILogger logger, int id, TimeSpan elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetProductByIdRequestSuccess)
            .Information(
                "Completed GET request for product {Id} in {ElapsedMs}ms",
                id, elapsedMs);

    private static void LogRequestFailed(ILogger logger, int id, TimeSpan elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetProductByIdRequestFailed)
            .Error("Failed to retrieve product with id {Id} in {ElapsedMs}ms", id, elapsedMs);
}

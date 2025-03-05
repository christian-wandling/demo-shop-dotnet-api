#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.Product.Queries.GetAllProducts;
using DemoShop.Domain.Common.Logging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ILogger = Serilog.ILogger;

#endregion

namespace DemoShop.Api.Features.Product.Endpoints;

[ApiVersion("1.0")]
public class GetAllProductsEndpoint(IMediator mediator, ILogger logger)
    : EndpointBaseAsync.WithoutRequest.WithResult<Result<ProductListResponse>>
{
    [TranslateResultToActionResult]
    [HttpGet("api/v{version:apiVersion}/products")]
    [SwaggerOperation(
        Summary = "Get all products",
        Description = "Get all products",
        OperationId = "GetAllProducts",
        Tags = ["Product"])
    ]
    public override async Task<Result<ProductListResponse>>
        HandleAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, "Get all products");

        var result = await mediator.Send(new GetAllProductsQuery(), cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, result.Value.Items.Count, stopwatch.Elapsed);
        else
            LogRequestFailed(logger, stopwatch.Elapsed);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetAllProductsRequestStarted)
            .Information("Starting GET request for all products at {Endpoint}", endpoint);

    private static void LogRequestSuccess(ILogger logger, int productCount, TimeSpan elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetAllProductsRequestSuccess)
            .Information(
                "Completed GET request for all products. Retrieved {ProductCount} products in {ElapsedMs}ms",
                productCount, elapsedMs);

    private static void LogRequestFailed(ILogger logger, TimeSpan elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetAllProductsRequestFailed)
            .Error("Failed to retrieve all products in {ElapsedMs}ms", elapsedMs);
}

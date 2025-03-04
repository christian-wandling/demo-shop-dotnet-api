#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Features.ShoppingSession.Commands.UpdateCartItemQuantity;
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
public class UpdateCartItemQuantityEndpoint(IMediator mediator, ILogger logger)
    : EndpointBaseAsync.WithRequest<UpdateCartItemQuantityRequestWrapper>.WithResult<
        Result<UpdateCartItemQuantityResponse>>
{
    [TranslateResultToActionResult]
    [HttpPatch("api/v{version:apiVersion}/shopping-sessions/current/cart-items/{id:int}")]
    [ExpectedFailures(
        ResultStatus.Unauthorized,
        ResultStatus.Forbidden,
        ResultStatus.Error,
        ResultStatus.NotFound,
        ResultStatus.Invalid
    )]
    [SwaggerOperation(
        Summary = "Update cart item quantity",
        Description =
            "Update quantity of a cart item in shopping session of current user based on identity extracted from bearer token",
        OperationId = "UpdateCartItemQuantity",
        Tags = ["ShoppingSession"])
    ]
    public override async Task<Result<UpdateCartItemQuantityResponse>> HandleAsync(
        [FromRoute] UpdateCartItemQuantityRequestWrapper request,
        CancellationToken cancellationToken = default
    )
    {
        Guard.Against.Null(request, nameof(request));
        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, "Update cart item quantity");

        var result = await mediator.Send(
            new UpdateCartItemQuantityCommand(request.Id, request.UpdateCartItemQuantityRequest),
            cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, request.Id, stopwatch.Elapsed);
        else
            LogRequestFailed(logger, stopwatch.Elapsed);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventIds.UpdateCartItemQuantityRequestStarted)
            .Information(
                "Starting PATCH request for updating quantity of cart item in current shopping session at {Endpoint}",
                endpoint);

    private static void LogRequestSuccess(ILogger logger, int id, TimeSpan elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventIds.UpdateCartItemQuantityRequestSuccess)
            .Information(
                "Completed PATCH request for updating quantity of cart item in current shopping session {Id} in {ElapsedMs}ms",
                id, elapsedMs);

    private static void LogRequestFailed(ILogger logger, TimeSpan elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventIds.UpdateCartItemQuantityRequestFailed)
            .Error(
                "Failed PATCH request to updating quantity of cart item in current shopping session in {ElapsedMs}ms",
                elapsedMs);
}

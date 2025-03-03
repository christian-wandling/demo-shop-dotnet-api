#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Processes.UserResolution;
using DemoShop.Domain.Common.Logging;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ILogger = Serilog.ILogger;

#endregion

namespace DemoShop.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class GetCurrentUserEndpoint(IMediator mediator, ILogger logger)
    : EndpointBaseAsync.WithoutRequest.WithResult<Result<UserResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.Error)]
    [HttpGet("api/v{version:apiVersion}/users/me")]
    [SwaggerOperation(
        Summary = "Get current user",
        Description = "Get current user based on identity extracted from bearer token",
        OperationId = "GetCurrentUser",
        Tags = ["User"])
    ]
    public override async Task<Result<UserResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, "Get current user");

        var result = await mediator.Send(new UserResolutionProcess(), cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, result.Value.Id, stopwatch.Elapsed);
        else
            LogRequestFailed(logger, stopwatch.Elapsed);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetCurrentUserRequestStarted)
            .Information("Starting GET request for user at {Endpoint}", endpoint);

    private static void LogRequestSuccess(ILogger logger, int id, TimeSpan elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetCurrentUserRequestSuccess)
            .Information(
                "Completed GET request for user {Id} in {ElapsedMs}ms",
                id, elapsedMs);

    private static void LogRequestFailed(ILogger logger, TimeSpan elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventIds.GetCurrentUserRequestFailed)
            .Error("Failed GET request to retrieve current user in {ElapsedMs}ms", elapsedMs);
}

#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Processes.ResolveUser;
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
public class ResolveCurrentUserEndpoint(IMediator mediator, ILogger logger)
    : EndpointBaseAsync.WithoutRequest.WithResult<Result<UserResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.Error)]
    [HttpPost("api/v{version:apiVersion}/users/me")]
    [SwaggerOperation(
        Summary = "Resolve current user",
        Description = "Resolve current user based on identity extracted from bearer token",
        OperationId = "ResolveCurrentUser",
        Tags = ["User"])
    ]
    public override async Task<Result<UserResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, "Resolve current user");

        var result = await mediator.Send(new ResolveUserProcess(), cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, result.Value.Id, stopwatch.Elapsed.Milliseconds);
        else
            LogRequestFailed(logger, stopwatch.Elapsed.Milliseconds);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventId.ResolveCurrentUserRequestStarted)
            .Debug("Starting POST request for resolving current user at {Endpoint}", endpoint);

    private static void LogRequestSuccess(ILogger logger, int id, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.ResolveCurrentUserRequestSuccess)
            .Information(
                "Completed POST request for resolving user {Id} in {ElapsedMs}ms",
                id, elapsedMs);

    private static void LogRequestFailed(ILogger logger, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.ResolveCurrentUserRequestFailed)
            .Error("Failed POST request to resolving current user in {ElapsedMs}ms", elapsedMs);
}

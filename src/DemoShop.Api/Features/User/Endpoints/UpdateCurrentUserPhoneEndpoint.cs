#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.User.Models;
using DemoShop.Application.Features.User.Commands.UpdateUserPhone;
using DemoShop.Application.Features.User.DTOs;
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
public class UpdateCurrentUserPhoneEndpoint(IMediator mediator, ILogger logger)
    : EndpointBaseAsync.WithRequest<UpdateUserPhoneRequest>.WithResult<Result<UserPhoneResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.Error, ResultStatus.Invalid)]
    [HttpPatch("api/v{version:apiVersion}/users/me/phone")]
    [SwaggerOperation(
        Summary = "Update phone of current user",
        Description = "Update the phone of the user based on identity extracted from bearer token",
        OperationId = "UpdateCurrentUserPhone",
        Tags = ["User"])
    ]
    public override async Task<Result<UserPhoneResponse>> HandleAsync(
        [FromBody] UpdateUserPhoneRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, "Update phone of current user");

        var result = await mediator.Send(new UpdateUserPhoneCommand(request), cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, stopwatch.Elapsed);
        else
            LogRequestFailed(logger, stopwatch.Elapsed);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventIds.UpdateUserPhoneRequestStarted)
            .Information("Starting PUT request for updating current user phone at {Endpoint}", endpoint);

    private static void LogRequestSuccess(ILogger logger, TimeSpan elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventIds.UpdateUserPhoneRequestSuccess)
            .Information(
                "Completed PUT request for updating current user phone in {ElapsedMs}ms", elapsedMs);

    private static void LogRequestFailed(ILogger logger, TimeSpan elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventIds.UpdateUserPhoneRequestFailed)
            .Error("Failed PUT request to update current user phone in {ElapsedMs}ms", elapsedMs);
}

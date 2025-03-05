#region

using System.Diagnostics;
using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.User.Models;
using DemoShop.Application.Features.User.Commands.UpdateUserAddress;
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
public class UpdateCurrentUserAddressEndpoint(IMediator mediator, ILogger logger)
    : EndpointBaseAsync.WithRequest<UpdateUserAddressRequest>.WithResult<Result<AddressResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.Error, ResultStatus.Invalid)]
    [HttpPut("api/v{version:apiVersion}/users/me/address")]
    [SwaggerOperation(
        Summary = "Update address of current user",
        Description = "Update the address of the user based on identity extracted from bearer token",
        OperationId = "UpdateCurrentUserAddress",
        Tags = ["User"])
    ]
    public override async Task<Result<AddressResponse>> HandleAsync(
        [FromBody] UpdateUserAddressRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var stopwatch = Stopwatch.StartNew();
        LogRequestStarting(logger, "Update address of current user");

        var result = await mediator.Send(new UpdateUserAddressCommand(request), cancellationToken);
        stopwatch.Stop();

        if (result.IsSuccess)
            LogRequestSuccess(logger, stopwatch.Elapsed.Milliseconds);
        else
            LogRequestFailed(logger, stopwatch.Elapsed.Milliseconds);

        return result;
    }

    private static void LogRequestStarting(ILogger logger, string endpoint) =>
        logger
            .ForContext("EventId", LoggerEventId.UpdateUserAddressRequestStarted)
            .Debug("Starting PUT request for updating current user address at {Endpoint}", endpoint);

    private static void LogRequestSuccess(ILogger logger, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.UpdateUserAddressRequestSuccess)
            .Information(
                "Completed PUT request for updating current user address in {ElapsedMs}ms", elapsedMs);

    private static void LogRequestFailed(ILogger logger, int elapsedMs) =>
        logger
            .ForContext("EventId", LoggerEventId.UpdateUserAddressRequestFailed)
            .Error("Failed PUT request to update current user address in {ElapsedMs}ms", elapsedMs);
}

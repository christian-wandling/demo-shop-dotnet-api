#region

using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Queries.GetOrCreateUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class GetCurrentUserEndpoint(IMediator mediator)
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
    public override async Task<Result<UserResponse>> HandleAsync(CancellationToken cancellationToken = default) =>
        await mediator.Send(new GetOrCreateUserQuery(), cancellationToken);
}

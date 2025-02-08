#region

using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.User.Models;
using DemoShop.Application.Features.User.Commands.UpdateUserPhone;
using DemoShop.Application.Features.User.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class UpdateCurrentUserPhoneEndpoint(IMediator mediator)
    : EndpointBaseAsync.WithRequest<UpdateUserPhoneRequest>.WithResult<Result<UserPhoneResponse>>
{
    [TranslateResultToActionResult]
    [ExpectedFailures(ResultStatus.Unauthorized, ResultStatus.Forbidden, ResultStatus.Error, ResultStatus.Invalid)]
    [HttpPatch("api/v{version:apiVersion}/users/me/phone")]
    [SwaggerOperation(
        Summary = "Update phone of current user",
        Description = "Update the phone of the user based on identity extracted from bearer token",
        OperationId = "User.UpdateCurrentUserPhone",
        Tags = ["User"])
    ]
    public override async Task<Result<UserPhoneResponse>> HandleAsync(
        [FromBody] UpdateUserPhoneRequest request,
        CancellationToken cancellationToken = default
    ) => await mediator.Send(new UpdateUserPhoneCommand(request), cancellationToken);
}

#region

using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using DemoShop.Api.Features.User.Models;
using DemoShop.Application.Features.User.Commands.UpdateUserAddress;
using DemoShop.Application.Features.User.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace DemoShop.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public class UpdateCurrentUserAddressEndpoint(IMediator mediator)
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
    ) => await mediator.Send(new UpdateUserAddressCommand(request), cancellationToken);
}

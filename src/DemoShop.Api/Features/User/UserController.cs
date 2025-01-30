using Ardalis.GuardClauses;
using Asp.Versioning;
using DemoShop.Api.Common;
using DemoShop.Api.Features.User.Models;
using DemoShop.Application.Features.User.Commands.UpdateUserAddress;
using DemoShop.Application.Features.User.Commands.UpdateUserPhone;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Queries.GetOrCreateUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoShop.Api.Features.User;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
[Route("api/v{version:apiVersion}/users/me")]
public sealed class UserController(IMediator mediator) : ApiController
{
    [HttpGet("")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var query = new GetOrCreateUserQuery();
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return ToActionResult(result);
    }

    [HttpPut("phone")]
    [ProducesResponseType(typeof(UserPhoneResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserPhoneResponse>> UpdateCurrentUserPhone(
        [FromBody] UpdateUserPhoneRequest updateUserPhone,
        CancellationToken cancellationToken
    )
    {
        Guard.Against.Null(updateUserPhone, nameof(updateUserPhone));

        var command = new UpdateUserPhoneCommand(updateUserPhone);
        var result = await mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return ToActionResult(result);
    }

    [HttpPut("address")]
    [ProducesResponseType(typeof(AddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AddressResponse?>> UpdateCurrentUserAddress(
        [FromBody] UpdateUserAddressRequest updateUserAddress,
        CancellationToken cancellationToken
    )
    {
        Guard.Against.Null(updateUserAddress, nameof(updateUserAddress));

        var command = new UpdateUserAddressCommand(updateUserAddress);
        var result = await mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return ToActionResult(result);
    }
}

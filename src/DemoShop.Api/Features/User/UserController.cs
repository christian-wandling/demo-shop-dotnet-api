using Ardalis.GuardClauses;
using Asp.Versioning;
using AutoMapper;
using DemoShop.Api.Common;
using DemoShop.Api.Features.User.Models;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Application.Features.User.Commands.UpdateUserPhone;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Queries.GetOrCreateUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoShop.Api.Features.User;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
[Route("api/v{version:apiVersion}/users")]
public sealed class UserController(
    IMediator mediator,
    IMapper mapper,
    ICurrentUserAccessor currentUser
) : ApiController
{
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdentity = currentUser.GetUserIdentity();
        if (!userIdentity.IsSuccess)
        {
            return Unauthorized();
        }

        var query = new GetOrCreateUserQuery(userIdentity.Value);
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return Ok(mapper.Map<UserResponse>(result.Value));
    }

    [HttpPut("me/phone")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCurrentUserAddress(
        [FromBody] UpdateUserPhoneRequest request,
        CancellationToken cancellationToken
    )
    {
        Guard.Against.Null(request, nameof(request));

        var userIdentity = currentUser.GetUserIdentity();
        if (!userIdentity.IsSuccess)
        {
            return Unauthorized();
        }

        var command = new UpdateUserPhoneCommand(userIdentity.Value, request.PhoneNumber);
        var result = await mediator.Send(command, cancellationToken).ConfigureAwait(false);

        return Ok(mapper.Map<UserResponse>(result.Value));
    }
}

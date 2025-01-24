using Asp.Versioning;
using AutoMapper;
using DemoShop.Api.Common;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Queries.GetOrCreateUser;
using DemoShop.Domain.Common.Logging;
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
    ILogger<UserController> logger,
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
            logger.LogAuthFailed(string.Join(", ", userIdentity.Errors));
            return Unauthorized();
        }

        var query = new GetOrCreateUserQuery(userIdentity.Value);
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return Ok(mapper.Map<UserResponse>(result.Value));
    }
}

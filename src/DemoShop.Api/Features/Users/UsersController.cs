using Asp.Versioning;
using AutoMapper;
using DemoShop.Api.Common;
using DemoShop.Api.Common.Logging;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Application.Features.Users.DTOs;
using DemoShop.Application.Features.Users.Queries.GetOrCreateUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoShop.Api.Features.Users;

[ApiVersion("1.0")]
[Authorize(Policy = "RequireBuyProductsRole")]
public sealed class UsersController(
    IMediator mediator,
    IMapper mapper,
    ILogger<UsersController> logger,
    ICurrentUserAccessor currentUser
) : ApiController
{
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        logger.LogGetCurrentUserStarted();

        var userIdentity = currentUser.GetUserIdentity();
        if (!userIdentity.IsSuccess)
        {
            logger.LogUserIdentityFailed(string.Join(", ", userIdentity.Errors));
            return Unauthorized();
        }

        var query = new GetOrCreateUserQuery(userIdentity.Value);
        var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

        logger.LogResult(result, "GetCurrentUser API endpoint");

        return Ok(mapper.Map<UserResponse>(result.Value));
    }
}

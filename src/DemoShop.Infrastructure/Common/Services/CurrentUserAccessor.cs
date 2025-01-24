using Ardalis.Result;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Application.Features.Common.Models;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DemoShop.Infrastructure.Common.Services;

public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, ILogger<UserIdentity> logger)
    : ICurrentUserAccessor
{
    public Result<IUserIdentity> GetUserIdentity()
    {
        var result = UserIdentity.FromClaimsPrincipal(httpContextAccessor.HttpContext?.User, logger);

        if (!result.IsSuccess)
        {
            logger.LogAuthFailed(string.Join(", ", result.Errors));
        }

        return result;
    }
}

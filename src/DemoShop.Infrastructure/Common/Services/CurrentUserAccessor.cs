using Ardalis.Result;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Application.Features.Common.Models;
using DemoShop.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DemoShop.Infrastructure.Common.Services;

public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, ILogger<UserIdentity> logger)
    : ICurrentUserAccessor
{
    public Result<IUserIdentity> GetUserIdentity() =>
        UserIdentity.FromClaimsPrincipal(httpContextAccessor.HttpContext?.User, logger);
}

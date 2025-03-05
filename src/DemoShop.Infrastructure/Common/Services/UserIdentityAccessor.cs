#region

using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Common.Models;
using DemoShop.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Common.Services;

public sealed class UserIdentityAccessor(IHttpContextAccessor httpContextAccessor, ILogger logger)
    : IUserIdentityAccessor
{
    public Result<IUserIdentity> GetCurrentIdentity() =>
        UserIdentity.FromClaimsPrincipal(httpContextAccessor.HttpContext?.User, logger);
}

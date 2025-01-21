using Ardalis.Result;
using DemoShop.Application.Features.Common.Models;
using DemoShop.Domain.Common.Interfaces;

namespace DemoShop.Application.Features.Common.Interfaces;

public interface ICurrentUserAccessor
{
    Result<IUserIdentity> GetUserIdentity();
}

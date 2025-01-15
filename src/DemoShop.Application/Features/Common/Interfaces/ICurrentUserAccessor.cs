using Ardalis.Result;
using DemoShop.Application.Features.Common.Models;

namespace DemoShop.Application.Features.Common.Interfaces;

public interface ICurrentUserAccessor
{
    Result<IUserIdentity> GetUserIdentity();
}

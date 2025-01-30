using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;

namespace DemoShop.Application.Features.Common.Interfaces;

public interface IUserIdentityAccessor
{
    Result<IUserIdentity> GetCurrentIdentity();
}

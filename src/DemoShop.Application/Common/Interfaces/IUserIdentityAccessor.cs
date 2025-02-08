#region

using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Application.Common.Interfaces;

public interface IUserIdentityAccessor
{
    Result<IUserIdentity> GetCurrentIdentity();
}

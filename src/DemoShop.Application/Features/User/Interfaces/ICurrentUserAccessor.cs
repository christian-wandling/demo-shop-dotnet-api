#region

using Ardalis.Result;

#endregion

namespace DemoShop.Application.Features.User.Interfaces;

public interface ICurrentUserAccessor
{
    Task<Result<int>> GetId(CancellationToken cancellationToken);
}

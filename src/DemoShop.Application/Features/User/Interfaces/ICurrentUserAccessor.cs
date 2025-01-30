using Ardalis.Result;

namespace DemoShop.Application.Features.User.Interfaces;

public interface ICurrentUserAccessor
{
    Task<Result<int>> GetId(CancellationToken cancellationToken);
}

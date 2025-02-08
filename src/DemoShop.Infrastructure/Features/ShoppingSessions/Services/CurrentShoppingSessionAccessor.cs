#region

using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;

#endregion

namespace DemoShop.Infrastructure.Features.ShoppingSessions.Services;

public sealed class CurrentShoppingSessionAccessor(
    IShoppingSessionRepository repository,
    ICurrentUserAccessor currentUser
)
    : ICurrentShoppingSessionAccessor
{
    public async Task<Result<ShoppingSessionEntity>> GetCurrent(CancellationToken cancellationToken)
    {
        var userResult = await currentUser.GetId(cancellationToken);

        if (!userResult.IsSuccess)
            return userResult.Map();

        var session = await repository.GetSessionByUserIdAsync(userResult.Value, cancellationToken);

        return session is null
            ? Result.NotFound("Session not found")
            : Result.Success(session);
    }
}

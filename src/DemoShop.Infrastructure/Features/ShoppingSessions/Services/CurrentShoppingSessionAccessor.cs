using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;

namespace DemoShop.Infrastructure.Features.ShoppingSessions.Services;

public sealed class CurrentShoppingSessionAccessor(
    IShoppingSessionRepository repository,
    ICurrentUserAccessor currentUser
)
    : ICurrentShoppingSessionAccessor
{
    public async Task<Result<ShoppingSessionEntity>> GetCurrent(CancellationToken cancellationToken)
    {
        var userResult = await currentUser.GetId(cancellationToken).ConfigureAwait(false);

        if (!userResult.IsSuccess) return Result.Forbidden("Authorization Failed");

        var session = await repository.GetSessionByUserIdAsync(userResult.Value, cancellationToken)
            .ConfigureAwait(false);

        Guard.Against.Null(session);

        return Result.Success(session);
    }
}

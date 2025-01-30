using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.User.Interfaces;

namespace DemoShop.Infrastructure.Features.Users.Service;

public sealed class CurrentUserAccessor(
    IUserRepository repository,
    IUserIdentityAccessor userIdentity
) : ICurrentUserAccessor
{
    public async Task<Result<int>> GetId(CancellationToken cancellationToken)
    {
        var identityResult = userIdentity.GetCurrentIdentity();

        if (!identityResult.IsSuccess)
        {
            return Result.Forbidden("Authorization Failed");
        }

        var user = await repository.GetUserByKeycloakIdAsync(identityResult.Value.KeycloakUserId, cancellationToken)
            .ConfigureAwait(false);

        Guard.Against.Null(user);

        return Result.Success(user.Id);
    }
}

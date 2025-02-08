#region

using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.User.Interfaces;

#endregion

namespace DemoShop.Infrastructure.Features.Users.Services;

public sealed class CurrentUserAccessor(
    IUserRepository repository,
    IUserIdentityAccessor userIdentity
) : ICurrentUserAccessor
{
    public async Task<Result<int>> GetId(CancellationToken cancellationToken)
    {
        var identityResult = userIdentity.GetCurrentIdentity();

        if (!identityResult.IsSuccess)
            return identityResult.Map();

        var user = await repository.GetUserByKeycloakIdAsync(identityResult.Value.KeycloakUserId, cancellationToken);

        return user is null
            ? Result.NotFound()
            : Result.Success(user.Id);
    }
}

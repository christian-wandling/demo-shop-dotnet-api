using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;

public sealed class GetUserByKeycloakIdQueryHandler(
    IUserRepository repository,
    ILogger<GetUserByKeycloakIdQueryHandler> logger)
    : IRequestHandler<GetUserByKeycloakIdQuery, Result<UserEntity>>
{
    public async Task<Result<UserEntity>> Handle(GetUserByKeycloakIdQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await repository.GetUserByKeycloakIdAsync(request.KeycloakId, cancellationToken).ConfigureAwait(false);

        if (user is not null)
            return Result<UserEntity>.Success(user);

        logger.LogOperationFailed("Get User By KeycloakId", "keycloakUserId", request.KeycloakId, null);
        return Result<UserEntity>.Error("User not found");
    }
}

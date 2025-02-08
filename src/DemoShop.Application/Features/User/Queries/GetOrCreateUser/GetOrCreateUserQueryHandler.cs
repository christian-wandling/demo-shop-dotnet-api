#region

using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.User.Queries.GetOrCreateUser;

public sealed class GetOrCreateUserQueryHandler(
    IUserIdentityAccessor identity,
    IMediator mediator,
    ILogger<GetOrCreateUserQueryHandler> logger
)
    : IRequestHandler<GetOrCreateUserQuery, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(GetOrCreateUserQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var identityResult = identity.GetCurrentIdentity();

        if (!identityResult.IsSuccess)
            return identityResult.Map<IUserIdentity, UserResponse>(null);

        try
        {
            var result = await mediator
                .Send(new GetUserByKeycloakIdQuery(identityResult.Value.KeycloakUserId), cancellationToken);

            if (!result.IsSuccess)
                result = await mediator.Send(new CreateUserCommand(identityResult.Value), cancellationToken);

            return result;
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            logger.LogOperationFailed("Get User By KeycloakUserId", "KeycloakUserId",
                identityResult.Value.KeycloakUserId, ex);
            return Result.Error(ex.Message);
        }
    }
}

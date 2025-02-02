using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;
using DemoShop.Domain.User.Entities;
using MediatR;

namespace DemoShop.Application.Features.User.Queries.GetOrCreateUser;

public sealed class GetOrCreateUserHandler(
    IUserIdentityAccessor identity,
    IMediator mediator
)
    : IRequestHandler<GetOrCreateUserQuery, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(GetOrCreateUserQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var identityResult = identity.GetCurrentIdentity();

        if (!identityResult.IsSuccess) return Result.Forbidden("Invalid identity");

        var result = await mediator
            .Send(new GetUserByKeycloakIdQuery(identityResult.Value.KeycloakUserId), cancellationToken)
            .ConfigureAwait(false);

        if (result.IsError())
            result = await mediator
                .Send(
                    new CreateUserCommand(identityResult.Value),
                    cancellationToken
                ).ConfigureAwait(false);

        return result;
    }
}

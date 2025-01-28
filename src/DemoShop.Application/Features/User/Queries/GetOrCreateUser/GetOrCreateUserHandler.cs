using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;
using DemoShop.Domain.User.Entities;
using MediatR;

namespace DemoShop.Application.Features.User.Queries.GetOrCreateUser;

public sealed class GetOrCreateUserHandler(IMediator mediator)
    : IRequestHandler<GetOrCreateUserQuery, Result<UserEntity>>
{
    public async Task<Result<UserEntity>> Handle(GetOrCreateUserQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Identity, nameof(request.Identity));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        var result = await mediator
            .Send(new GetUserByKeycloakIdQuery(request.Identity.KeycloakUserId), cancellationToken)
            .ConfigureAwait(false);

        if (result.IsError())
        {
            result = await mediator
                .Send(
                    new CreateUserCommand(request.Identity),
                    cancellationToken
                ).ConfigureAwait(false);
        }

        return result;
    }
}

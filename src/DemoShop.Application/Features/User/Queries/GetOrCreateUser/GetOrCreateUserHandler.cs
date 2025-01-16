using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.Application.Features.User.Logging;
using DemoShop.Application.Features.User.Queries.GetUserByEmail;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Queries.GetOrCreateUser;

public sealed class GetOrCreateUserHandler(IMediator mediator, ILogger<GetOrCreateUserHandler> logger)
    : IRequestHandler<GetOrCreateUserQuery, Result<Domain.User.Entities.UserEntity>>
{
    public async Task<Result<Domain.User.Entities.UserEntity>> Handle(GetOrCreateUserQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        logger.LogUserGetOrCreateStarted(request.Identity.Email);

        var user = await mediator
            .Send(new GetUserByEmailQuery(request.Identity.Email), cancellationToken)
            .ConfigureAwait(false);

        if (user is not null) return Result<Domain.User.Entities.UserEntity>.Success(user);

        user = await mediator
            .Send(
                new CreateUserCommand(request.Identity.KeycloakId, request.Identity.Email, request.Identity.FirstName,
                    request.Identity.LastName),
                cancellationToken
            ).ConfigureAwait(false);

        return Result<Domain.User.Entities.UserEntity>.Success(user);
    }
}

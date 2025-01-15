using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.Users.Commands.CreateUser;
using DemoShop.Application.Features.Users.Queries.GetUserByEmail;
using DemoShop.Application.Features.Users.Logging;
using DemoShop.Domain.Users.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Users.Queries.GetOrCreateUser;

public sealed class GetOrCreateUserHandler(IMediator mediator, ILogger<GetOrCreateUserHandler> logger)
    : IRequestHandler<GetOrCreateUserQuery, Result<User>>
{
    public async Task<Result<User>> Handle(GetOrCreateUserQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        logger.LogUserGetOrCreateStarted(request.Identity.Email);

        var user = await mediator
            .Send(new GetUserByEmailQuery(request.Identity.Email), cancellationToken)
            .ConfigureAwait(false);

        if (user is not null) return Result<User>.Success(user);

        user = await mediator
            .Send(
                new CreateUserCommand(request.Identity.KeycloakId, request.Identity.Email, request.Identity.FirstName,
                    request.Identity.LastName),
                cancellationToken
            ).ConfigureAwait(false);

        return Result<User>.Success(user);
    }
}

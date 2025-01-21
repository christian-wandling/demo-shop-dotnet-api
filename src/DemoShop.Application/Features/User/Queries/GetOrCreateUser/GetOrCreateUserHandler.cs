using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.Application.Features.User.Logging;
using DemoShop.Application.Features.User.Queries.GetUserByEmail;
using DemoShop.Domain.User.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Queries.GetOrCreateUser;

public sealed class GetOrCreateUserHandler(IMediator mediator, ILogger<GetOrCreateUserHandler> logger)
    : IRequestHandler<GetOrCreateUserQuery, Result<UserEntity>>
{
    public async Task<Result<UserEntity>> Handle(GetOrCreateUserQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Identity, nameof(request.Identity));
        Guard.Against.Null(cancellationToken, nameof(cancellationToken));

        logger.LogUserGetOrCreateStarted(request.Identity.Email);

        var result = await mediator
            .Send(new GetUserByEmailQuery(request.Identity.Email), cancellationToken)
            .ConfigureAwait(false);

        if (result.IsError())
        {
            result = await mediator
                .Send(
                    new CreateUserCommand(request.Identity.KeycloakId, request.Identity.Email,
                        request.Identity.FirstName,
                        request.Identity.LastName),
                    cancellationToken
                ).ConfigureAwait(false);
        }

        return result;
    }
}

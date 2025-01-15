using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.Users.Logging;
using DemoShop.Domain.Users.Entities;
using DemoShop.Domain.Users.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Users.Queries.GetUserByEmail;

public sealed class GetUserByEmailQueryHandler(IUserRepository repository, ILogger<GetUserByEmailQueryHandler> logger)
    : IRequestHandler<GetUserByEmailQuery, Result<User>?>
{
    public async Task<Result<User>?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await repository.GetUserByEmailAsync(request.Email, cancellationToken).ConfigureAwait(false);

        if (user is null)
        {
            logger.LogUserNotFound(request.Email);
            return Result<User>.Error("Failed to create user");
        }

        logger.LogUserFound($"{user.Id}");
        return Result<User>.Success(user);
    }
}

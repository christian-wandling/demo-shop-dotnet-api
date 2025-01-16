using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Application.Features.User.Logging;
using DemoShop.Domain.User.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Queries.GetUserByEmail;

public sealed class GetUserByEmailQueryHandler(IUserRepository repository, ILogger<GetUserByEmailQueryHandler> logger)
    : IRequestHandler<GetUserByEmailQuery, Result<Domain.User.Entities.UserEntity>?>
{
    public async Task<Result<Domain.User.Entities.UserEntity>?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await repository.GetUserByEmailAsync(request.Email, cancellationToken).ConfigureAwait(false);

        if (user is null)
        {
            logger.LogUserNotFound(request.Email);
            return Result<Domain.User.Entities.UserEntity>.Error("Failed to create user");
        }

        logger.LogUserFound($"{user.Id}");
        return Result<Domain.User.Entities.UserEntity>.Success(user);
    }
}

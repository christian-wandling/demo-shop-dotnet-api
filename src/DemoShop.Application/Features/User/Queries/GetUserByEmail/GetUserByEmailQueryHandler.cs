using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Queries.GetUserByEmail;

public sealed class GetUserByEmailQueryHandler(IUserRepository repository, ILogger<GetUserByEmailQueryHandler> logger)
    : IRequestHandler<GetUserByEmailQuery, Result<UserEntity>>
{
    public async Task<Result<UserEntity>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await repository.GetUserByEmailAsync(request.Email, cancellationToken).ConfigureAwait(false);

        if (user is not null)
            return Result<UserEntity>.Success(user);

        logger.LogOperationFailed("Get User By Email", "email", request.Email, null);
        return Result<UserEntity>.Error("User not found");
    }
}

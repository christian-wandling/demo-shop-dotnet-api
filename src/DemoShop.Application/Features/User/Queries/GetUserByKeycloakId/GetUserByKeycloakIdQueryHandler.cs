using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;

public sealed class GetUserByKeycloakIdQueryHandler(
    IMapper mapper,
    IUserRepository repository,
    ILogger<GetUserByKeycloakIdQueryHandler> logger)
    : IRequestHandler<GetUserByKeycloakIdQuery, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(GetUserByKeycloakIdQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await repository.GetUserByKeycloakIdAsync(request.KeycloakId, cancellationToken)
            .ConfigureAwait(false);

        if (user is not null)
            return Result.Success(mapper.Map<UserResponse>(user));

        logger.LogOperationFailed("Get User By KeycloakUserId", "KeycloakUserId", request.KeycloakId, null);
        return Result.Error("User not found");
    }
}

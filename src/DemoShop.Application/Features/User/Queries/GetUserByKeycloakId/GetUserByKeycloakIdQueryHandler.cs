#region

using System.Data.Common;
using Ardalis.GuardClauses;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;

public sealed class GetUserByKeycloakIdQueryHandler(
    IMapper mapper,
    IUserRepository repository,
    ILogger<GetUserByKeycloakIdQueryHandler> logger)
    : IRequestHandler<GetUserByKeycloakIdQuery, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(GetUserByKeycloakIdQuery request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        try
        {
            var user = await repository.GetUserByKeycloakIdAsync(request.KeycloakId, cancellationToken);

            if (user is not null)
                return Result.Success(mapper.Map<UserResponse>(user));

            logger.LogOperationFailed("Get User By KeycloakUserId", "KeycloakUserId", request.KeycloakId, null);
            return Result.Error("User not found");
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            logger.LogOperationFailed("Get User By KeycloakUserId", "KeycloakUserId", request.KeycloakId, ex);
            return Result.Error(ex.Message);
        }
    }
}

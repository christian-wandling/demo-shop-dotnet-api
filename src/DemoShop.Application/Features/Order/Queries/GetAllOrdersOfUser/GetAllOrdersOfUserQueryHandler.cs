#region

using System.Data.Common;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Interfaces;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Features.Order.Queries.GetAllOrdersOfUser;

public sealed class GetAllOrdersOfUserQueryHandler(
    ICurrentUserAccessor user,
    IMapper mapper,
    IOrderRepository repository,
    ILogger logger,
    ICacheService cacheService
)
    : IRequestHandler<GetAllOrdersOfUserQuery, Result<OrderListResponse>>
{
    public async Task<Result<OrderListResponse>> Handle(GetAllOrdersOfUserQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdResult = await user.GetId(cancellationToken);
            if (!userIdResult.IsSuccess)
                return Result.Unauthorized("Authorization Failed");

            LogQueryStarted(logger, userIdResult.Value);

            var cacheKey = cacheService.GenerateCacheKey("orders-of-user", userIdResult.Value);
            var response = cacheService.GetFromCache<OrderListResponse>(cacheKey)
                           ?? await GetFromDatabase(userIdResult.Value, cacheKey, cancellationToken);

            LogQuerySuccess(logger, userIdResult.Value, response.Items.Count);
            return Result.Success(response);
        }
        catch (InvalidOperationException ex)
        {
            LogInvalidOperationException(logger, ex.Message, ex);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            LogDatabaseException(logger, ex.Message, ex);
            return Result.Error(ex.Message);
        }
    }

    private async Task<OrderListResponse> GetFromDatabase(int userId, string cacheKey,
        CancellationToken cancellationToken)
    {
        var orders = await repository.GetOrdersByUserIdAsync(userId, cancellationToken);
        var response = mapper.Map<OrderListResponse>(orders);

        if (response.Items.Count > 0)
            cacheService.SetCache(cacheKey, response);

        return response;
    }

    private static void LogQueryStarted(ILogger logger, int userId) => logger
        .ForContext("EventId", LoggerEventId.GetAllOrdersOfUserQueryStarted)
        .Information(
            "Starting query to retrieve all orders for user {UserId}", userId);

    private static void LogQuerySuccess(ILogger logger, int userId, int count) => logger
            .ForContext("EventId", LoggerEventId.GetAllOrdersOfUserQuerySuccess)
            .Information("Successfully retrieved {Count} orders for user {UserId}", userId, count);

    private static void LogDatabaseException(ILogger logger, string errorMessage, Exception ex) => logger
            .ForContext("EventId", LoggerEventId.GetOrdersByUserIdDatabaseException)
            .Error(ex, "Database error occurred while retrieving all products. Error: {ErrorMessage}", errorMessage);

    private static void LogInvalidOperationException(ILogger logger, string errorMessage, Exception ex) => logger
            .ForContext("EventId", LoggerEventId.GetAllOrdersOfUserDomainException)
            .Error(ex, "Invalid operation while retrieving all products. Error: {ErrorMessage}", errorMessage);
}

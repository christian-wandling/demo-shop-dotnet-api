#region

using System.Data.Common;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.Order.Queries.GetAllOrdersOfUser;

public sealed class GetAllOrdersOfUserQueryHandler(
    ICurrentUserAccessor user,
    IMapper mapper,
    IOrderRepository repository,
    ILogger<GetAllOrdersOfUserQueryHandler> logger
)
    : IRequestHandler<GetAllOrdersOfUserQuery, Result<OrderListResponse>>
{
    public async Task<Result<OrderListResponse>> Handle(GetAllOrdersOfUserQuery request,
        CancellationToken cancellationToken)
    {
        var userIdResult = await user.GetId(cancellationToken);

        if (!userIdResult.IsSuccess) return Result.Unauthorized("Authorization Failed");

        try
        {
            var result = await repository.GetOrdersByUserIdAsync(userIdResult.Value, cancellationToken);

            return Result.Success(mapper.Map<OrderListResponse>(result));
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDomainException(ex.Message);
            return Result.Error(ex.Message);
        }
        catch (DbException ex)
        {
            logger.LogOperationFailed("Get all orders of user", "UserId", $"{userIdResult.Value}", ex);
            return Result.Error(ex.Message);
        }
    }
}

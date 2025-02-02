using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.Product.Queries.GetAllProducts;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Order.Interfaces;
using DemoShop.Domain.Product.Interfaces;
using DemoShop.Domain.User.Interfaces;
using MediatR;

namespace DemoShop.Application.Features.Order.Queries.GetAllOrdersOfUser;

public sealed class GetAllOrdersOfUserHandler(
    ICurrentUserAccessor user,
    IMapper mapper,
    IOrderRepository repository
)
    : IRequestHandler<GetAllOrdersOfUserQuery, Result<OrderListResponse>>
{
    public async Task<Result<OrderListResponse>> Handle(GetAllOrdersOfUserQuery request,
        CancellationToken cancellationToken)
    {
        var userIdResult = await user.GetId(cancellationToken).ConfigureAwait(false);

        if (!userIdResult.IsSuccess) return Result.Forbidden("Authorization Failed");

        var result = await repository.GetOrdersByUserIdAsync(userIdResult.Value, cancellationToken)
            .ConfigureAwait(false);

        return Result<OrderListResponse>.Success(mapper.Map<OrderListResponse>(result));
    }
}

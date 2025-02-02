using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Interfaces;
using MediatR;

namespace DemoShop.Application.Features.Order.Commands.ConvertShoppingSessionToOrder;

public sealed class ConvertShoppingSessionToOrderHandler(IOrderRepository orderRepository)
    : IRequestHandler<ConvertShoppingSessionToOrderCommand, Result<OrderEntity?>>
{
    public async Task<Result<OrderEntity?>> Handle(ConvertShoppingSessionToOrderCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Session, nameof(request.Session));

        var result = request.Session.ConvertToOrder();
        var createdOrder = await orderRepository.CreateOrderAsync(result.Value, cancellationToken)
            .ConfigureAwait(false);

        return Result.Success(createdOrder);
    }
}

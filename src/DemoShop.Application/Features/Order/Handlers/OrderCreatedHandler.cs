using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Order.Handlers;

public class OrderCreatedHandler(ILogger<OrderCreatedHandler> logger)
    : INotificationHandler<OrderCreatedDomainEvent>
{
    public Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.Order, nameof(notification.Order));

        logger.LogOperationSuccess("Create Order", "id", $"{notification.Order.Id}");
        return Task.CompletedTask;
    }
}

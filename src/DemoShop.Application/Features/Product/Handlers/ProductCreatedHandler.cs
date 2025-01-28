using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductCreatedHandler(ILogger<ProductCreatedHandler> logger)
    : INotificationHandler<ProductCreatedDomainEvent>
{
    public Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.Product, nameof(notification.Product));

        logger.LogOperationSuccess("Create Product", "id", $"{notification.Product.Id}");
        return Task.CompletedTask;
    }
}

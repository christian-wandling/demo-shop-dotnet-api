using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductRestoredHandler(ILogger<ProductRestoredHandler> logger)
    : INotificationHandler<ProductRestoredDomainEvent>
{
    public Task Handle(ProductRestoredDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        logger.LogOperationSuccess("Restore Product", "id",$"{notification.Id}");
        return Task.CompletedTask;
    }
}

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductCategoryRestoredHandler(ILogger<ProductCategoryRestoredHandler> logger)
    : INotificationHandler<CategoryRestoredDomainEvent>
{
    public Task Handle(CategoryRestoredDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        logger.LogOperationSuccess("Restore Category", "id", $"{notification.Id}");

        return Task.CompletedTask;
    }
}

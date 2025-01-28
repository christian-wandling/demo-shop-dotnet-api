using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductCategoryCreatedHandler(ILogger<ProductCategoryCreatedHandler> logger)
    : INotificationHandler<CategoryCreatedDomainEvent>
{
    public Task Handle(CategoryCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.Null(notification.Category, nameof(notification.Category));

        logger.LogOperationSuccess("Create Category", "id", $"{notification.Category.Id}");

        return Task.CompletedTask;
    }
}

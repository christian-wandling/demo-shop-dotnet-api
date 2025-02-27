#region

using Ardalis.GuardClauses;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Events;
using MediatR;
using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Application.Features.Product.Handlers;

public class ProductCategoryDeletedHandler(ILogger<ProductCategoryDeletedHandler> logger)
    : INotificationHandler<CategoryDeletedDomainEvent>
{
    public Task Handle(CategoryDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.Id, nameof(notification.Id));

        logger.LogOperationSuccess("Delete Category", "id", $"{notification.Id}");

        return Task.CompletedTask;
    }
}

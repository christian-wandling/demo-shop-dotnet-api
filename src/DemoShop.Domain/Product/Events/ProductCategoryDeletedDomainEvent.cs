#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Product.Events;

public record ProductCategoryDeletedDomainEvent(int Id, IReadOnlyCollection<int> ProductIds)
    : DomainEvent, IDomainEvent;

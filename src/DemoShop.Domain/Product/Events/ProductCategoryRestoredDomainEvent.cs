#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Product.Events;

public record ProductCategoryRestoredDomainEvent(int Id) : DomainEvent, IDomainEvent;

#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Product.Events;

public record ProductCreatedDomainEvent(int Id) : DomainEvent, IDomainEvent;

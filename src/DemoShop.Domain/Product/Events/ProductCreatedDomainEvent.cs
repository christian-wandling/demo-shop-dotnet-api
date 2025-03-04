#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Product.Entities;

#endregion

namespace DemoShop.Domain.Product.Events;

public record ProductCreatedDomainEvent(int Id) : DomainEvent, IDomainEvent;

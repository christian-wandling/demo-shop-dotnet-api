#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Product.Events;

public record ImageDeletedDomainEvent(int Id, int ProductId) : DomainEvent, IDomainEvent;

#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Order.Events;

public record OrderItemDeletedDomainEvent(int Id, int OrderId, int UserId) : DomainEvent, IDomainEvent;

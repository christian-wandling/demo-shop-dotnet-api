#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Order.Events;

public record OrderCreatedDomainEvent(int Id, int UserId) : DomainEvent, IDomainEvent;

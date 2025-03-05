#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.ShoppingSession.Events;

public sealed record CartItemAdded(int Id, int UserId) : DomainEvent, IDomainEvent;

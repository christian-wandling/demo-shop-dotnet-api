#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.User.Events;

public record UserRestoredDomainEvent(int Id) : DomainEvent, IDomainEvent;

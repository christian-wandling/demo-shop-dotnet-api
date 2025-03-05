#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.Entities;

#endregion

namespace DemoShop.Domain.User.Events;

public record UserCreatedDomainEvent(UserEntity User) : DomainEvent, IDomainEvent;

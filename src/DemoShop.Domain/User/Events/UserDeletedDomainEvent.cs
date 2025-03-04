#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.User.Events;

public record UserDeletedDomainEvent(int Id, string KeycloakUserId) : DomainEvent, IDomainEvent;

#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.ValueObjects;

#endregion

namespace DemoShop.Domain.User.Events;

public record UserPhoneUpdatedDomainEvent(
    int Id,
    string KeycloakUserId,
    Phone NewPhone,
    Phone OldPhone
) : DomainEvent, IDomainEvent;

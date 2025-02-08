#region

using System.Runtime.CompilerServices;
using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.Domain.Common.Base;

public static class DomainEventExtensions
{
    private static readonly ConditionalWeakTable<IEntity, List<IDomainEvent>> DomainEvents = [];

    public static IReadOnlyCollection<IDomainEvent> GetDomainEvents(this IEntity entity)
    {
        var events = DomainEvents.GetOrCreateValue(entity);
        return events.AsReadOnly();
    }

    public static void AddDomainEvent(this IEntity entity, IDomainEvent domainEvent)
    {
        var events = DomainEvents.GetOrCreateValue(entity);
        events.Add(domainEvent);
    }

    public static void ClearDomainEvents(this IEntity entity)
    {
        var events = DomainEvents.GetOrCreateValue(entity);
        events.Clear();
    }
}

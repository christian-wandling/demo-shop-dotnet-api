using DemoShop.Domain.Common.Interfaces;

namespace DemoShop.Domain.Session.Events;

public class SessionAddedDomainEvent(int userId, int sessionId ) : IDomainEvent
{
    public int UserId { get; } = userId;
    public int SessionId { get; } = sessionId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

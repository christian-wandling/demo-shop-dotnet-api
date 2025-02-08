#region

using Microsoft.Extensions.Logging;

#endregion

namespace DemoShop.Domain.Common.Logging;

public static class LoggerEventIds
{
    public static readonly EventId OperationSuccess = new(0, "OperationSuccess");
    public static readonly EventId DomainEvent = new(1, "DomainEvent");
    public static readonly EventId OperationFailed = new(100, "OperationFailed");
    public static readonly EventId DomainException = new(101, "DomainEvent");
    public static readonly EventId AuthFailed = new(102, "AuthFailed");
    public static readonly EventId ValidationFailed = new(103, "ValidationFailed");
}

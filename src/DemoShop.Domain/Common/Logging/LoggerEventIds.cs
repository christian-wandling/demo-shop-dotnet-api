using Microsoft.Extensions.Logging;

namespace DemoShop.Domain.Common.Logging;

public static class LoggerEventIds
{
    public static readonly EventId OperationSuccess = new(0, "OperationSuccess");
    public static readonly EventId OperationFailed = new(100, "OperationFailed");
    public static readonly EventId AuthFailed = new(101, "AuthFailed");
    public static readonly EventId ValidationFailed = new(102, "ValidationFailed");
}

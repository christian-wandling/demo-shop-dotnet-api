#region

using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.Common.Exceptions;

public class NotMarkedAsDeletedException : DomainException
{
    public NotMarkedAsDeletedException(string message) : base(message)
    {
    }

    public NotMarkedAsDeletedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public NotMarkedAsDeletedException()
    {
    }
}

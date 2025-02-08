#region

using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.Common.Exceptions;

public class AlreadyMarkedAsDeletedException : DomainException

{
    public AlreadyMarkedAsDeletedException(string message) : base(message)
    {
    }

    public AlreadyMarkedAsDeletedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public AlreadyMarkedAsDeletedException()
    {
    }
}

#region

using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.Common.Exceptions;

public class AlreadyExistsException : DomainException
{
    public AlreadyExistsException(string message) : base(message)
    {
    }

    public AlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public AlreadyExistsException()
    {
    }
}

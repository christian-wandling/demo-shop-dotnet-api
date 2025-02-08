#region

using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.Common.Exceptions;

public class DatabaseOperationException : DomainException
{
    public DatabaseOperationException(string message) : base(message)
    {
    }

    public DatabaseOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public DatabaseOperationException()
    {
    }
}

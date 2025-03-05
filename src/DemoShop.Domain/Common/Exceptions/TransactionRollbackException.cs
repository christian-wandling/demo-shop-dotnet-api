#region

using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.Common.Exceptions;

public class TransactionRollbackException : DomainException
{
    public TransactionRollbackException()
    {
    }

    public TransactionRollbackException(string message) : base(message)
    {
    }

    public TransactionRollbackException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

using DemoShop.Domain.Common.Base;

namespace DemoShop.Domain.Common.Exceptions;

public class TransactionRollbackException : DomainException
{
    public TransactionRollbackException() : base()
    {
    }

    public TransactionRollbackException(string message) : base(message)
    {
    }

    public TransactionRollbackException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

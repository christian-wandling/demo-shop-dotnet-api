namespace DemoShop.Domain.Common.Exceptions;

public class DatabaseOperationException : Exception
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

namespace DemoShop.Domain.Common.Exceptions;

public class AlreadyMarkedAsDeletedException : Exception
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

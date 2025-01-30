namespace DemoShop.Domain.Common.Exceptions;

public class NotMarkedAsDeletedException : Exception
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

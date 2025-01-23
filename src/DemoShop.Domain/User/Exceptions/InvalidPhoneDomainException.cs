using DemoShop.Domain.Common.Exceptions;

namespace DemoShop.Domain.User.Exceptions;

public class InvalidPhoneDomainException : DomainException
{
    public InvalidPhoneDomainException()
    {
    }

    public InvalidPhoneDomainException(string message)
        : base(message)
    {
    }


    public InvalidPhoneDomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

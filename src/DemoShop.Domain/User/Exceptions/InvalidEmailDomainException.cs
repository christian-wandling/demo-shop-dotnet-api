using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Exceptions;

namespace DemoShop.Domain.User.Exceptions;

public class InvalidEmailDomainException : DomainException
{
    public InvalidEmailDomainException()
    {
    }

    public InvalidEmailDomainException(string message)
        : base(message)
    {
    }


    public InvalidEmailDomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

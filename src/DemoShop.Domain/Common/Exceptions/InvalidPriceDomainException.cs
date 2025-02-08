#region

using DemoShop.Domain.Common.Base;

#endregion

namespace DemoShop.Domain.Common.Exceptions;

public class InvalidPriceDomainException : DomainException
{
    public InvalidPriceDomainException()
    {
    }

    public InvalidPriceDomainException(string message)
        : base(message)
    {
    }


    public InvalidPriceDomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

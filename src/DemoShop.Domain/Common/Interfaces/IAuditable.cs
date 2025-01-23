using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.Common.Interfaces;

public interface IAuditable
{
    Audit Audit { get; }
}

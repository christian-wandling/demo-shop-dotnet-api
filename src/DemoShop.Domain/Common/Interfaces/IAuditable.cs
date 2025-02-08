#region

using DemoShop.Domain.Common.ValueObjects;

#endregion

namespace DemoShop.Domain.Common.Interfaces;

public interface IAuditable
{
    Audit Audit { get; }
}

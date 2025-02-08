#region

using DemoShop.Domain.Common.ValueObjects;

#endregion

namespace DemoShop.Domain.Common.Interfaces;

public interface ISoftDeletable
{
    SoftDelete SoftDelete { get; }
}

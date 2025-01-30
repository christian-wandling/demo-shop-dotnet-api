using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.Common.Interfaces;

public interface ISoftDeletable
{
    SoftDelete SoftDelete { get; }
}

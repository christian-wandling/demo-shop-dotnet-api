namespace DemoShop.Domain.Common.Interfaces;

public interface IEntitySoftDelete
{
    bool Deleted { get; }
    DateTime? DeletedAt { get; }
}

namespace DemoShop.Domain.Common.Interfaces;

public interface IEntity
{
    int Id { get; }
    DateTime CreatedAt { get; }
    DateTime ModifiedAt { get; }
}

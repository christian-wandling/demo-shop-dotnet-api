#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;

#endregion

namespace DemoShop.Domain.Product.Entities;

public class CategoryEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    private readonly List<ProductEntity> _products = [];

    private CategoryEntity()
    {
        Name = string.Empty;
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    private CategoryEntity(string name)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    public string Name { get; private init; }
    public IReadOnlyCollection<ProductEntity> Products => _products.AsReadOnly();
    public Audit Audit { get; }

    public int Id { get; }
    public SoftDelete SoftDelete { get; }

    public static Result<CategoryEntity> Create(string name)
    {
        var category = new CategoryEntity(name);

        return Result.Success(category);
    }
}

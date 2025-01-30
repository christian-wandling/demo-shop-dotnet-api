using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Exceptions;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Product.Events;
using DemoShop.Domain.User.Events;
using DemoShop.Domain.User.ValueObjects;

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

    public int Id { get; }
    public string Name { get; private init; }
    public IReadOnlyCollection<ProductEntity> Products => _products.AsReadOnly();
    public Audit Audit { get; }
    public SoftDelete SoftDelete { get; }

    public static Result<CategoryEntity> Create(string name)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        var category = new CategoryEntity(name);
        return Result.Success(category);
    }

    public void Delete()
    {
        if (SoftDelete.Deleted)
        {
            throw new AlreadyMarkedAsDeletedException($"Category {Id} has already been marked as deleted");
        }

        SoftDelete.MarkAsDeleted();
        Audit.UpdateModified();
        this.AddDomainEvent(new CategoryDeletedDomainEvent(Id));
    }

    public void Restore()
    {
        if (SoftDelete.Deleted)
        {
            throw new NotMarkedAsDeletedException($"Category {Id} has not been marked as deleted");
        }

        SoftDelete.Restore();
        Audit.UpdateModified();
        this.AddDomainEvent(new CategoryRestoredDomainEvent(Id));
    }
}

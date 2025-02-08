#region

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
using DemoShop.Domain.Product.Events;
using DemoShop.Domain.ShoppingSession.Entities;

#endregion

namespace DemoShop.Domain.Product.Entities;

public class ProductEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    private readonly List<CartItemEntity> _cartItems = [];
    private readonly List<CategoryEntity> _categories = [];
    private readonly List<ImageEntity> _images = [];

    private ProductEntity()
    {
        Name = string.Empty;
        Description = string.Empty;
        Price = Price.Empty;
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    private ProductEntity(string name, string description, decimal price)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
        Price = Price.Create(price);
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Price Price { get; private set; }

    public IReadOnlyCollection<CategoryEntity> Categories => _categories.AsReadOnly();
    public IReadOnlyCollection<ImageEntity> Images => _images.AsReadOnly();
    public IReadOnlyCollection<CartItemEntity> CartItems => _cartItems.AsReadOnly();

    public string? Thumbnail => _images.FirstOrDefault()?.Uri.ToString();
    public Audit Audit { get; }

    public int Id { get; }
    public SoftDelete SoftDelete { get; }

    public static Result<ProductEntity> Create(string name, string description, decimal price)
    {
        var product = new ProductEntity(name, description, price);
        product.AddDomainEvent(new ProductCreatedDomainEvent(product));
        return Result.Success(product);
    }
}

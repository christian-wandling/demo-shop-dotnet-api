using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.Product.Entities;

public class ProductEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    private readonly List<CategoryEntity> _categories = [];
    private readonly List<ImageEntity> _images = [];
    private readonly List<CartItemEntity> _cartItems = [];

    public ProductEntity()
    {
        Name = string.Empty;
        Description = string.Empty;
        Price = 0;
        Audit = Audit.Create();
        SoftDeleteAudit = SoftDeleteAudit.Create();
    }

    public ProductEntity(string name, string description, decimal price)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
        Price = Guard.Against.NegativeOrZero(price, nameof(price));
        Audit = Audit.Create();
        SoftDeleteAudit = SoftDeleteAudit.Create();
    }

    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }

    public IReadOnlyCollection<CategoryEntity> Categories => _categories.AsReadOnly();
    public IReadOnlyCollection<ImageEntity> Images => _images.AsReadOnly();
    public IReadOnlyCollection<CartItemEntity> CartItems => _cartItems.AsReadOnly();

    public int Id { get; }
    public Audit Audit { get; set; }
    public SoftDeleteAudit SoftDeleteAudit { get; set; }

    public static Result<ProductEntity> Create(string name, string description, decimal price)
    {
        var product = new ProductEntity(name, description, price);
        return Result.Success(product);
    }
}

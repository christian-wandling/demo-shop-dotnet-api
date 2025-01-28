using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Product.Events;
using DemoShop.Domain.Product.ValueObjects;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.Product.Entities;

public class ProductEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    private readonly List<CategoryEntity> _categories = [];
    private readonly List<ImageEntity> _images = [];
    private readonly List<CartItemEntity> _cartItems = [];

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

    public int Id { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Price Price { get; private set; }
    public Audit Audit { get; }
    public SoftDelete SoftDelete { get; }

    public IReadOnlyCollection<CategoryEntity> Categories => _categories.AsReadOnly();
    public IReadOnlyCollection<ImageEntity> Images => _images.AsReadOnly();
    public IReadOnlyCollection<CartItemEntity> CartItems => _cartItems.AsReadOnly();

    public static Result<ProductEntity> Create(string name, string description, decimal price)
    {
        var product = new ProductEntity(name, description, price);
        product.AddDomainEvent(new ProductCreatedDomainEvent(product));
        return Result.Success(product);
    }

    public Result Delete()
    {
        if (SoftDelete.Deleted)
            return Result.Error("Product is already deleted");

        SoftDelete.MarkAsDeleted();
        this.AddDomainEvent(new ProductDeletedDomainEvent(Id));

        return Result.Success();
    }

    public Result Restore()
    {
        if (!SoftDelete.Deleted)
            return Result.Error("Product is not deleted");

        SoftDelete.MarkAsDeleted();
        this.AddDomainEvent(new ProductRestoredDomainEvent(Id));

        return Result.Success();
    }

    public Result AddCategory(CategoryEntity category)
    {
        Guard.Against.Null(category, nameof(category));

        if (_categories.Any(c => c.Id == category.Id))
        {
            return Result.Error("Category already exists");
        }

        _categories.Add(category);
        return Result.Success();
    }

    public Result RemoveCategory(CategoryEntity category)
    {
        Guard.Against.Null(category, nameof(category));

        if (!_categories.Contains(category))
            throw new NotFoundException(nameof(category), "Shopping category not found");

        _categories.Remove(category);
        return Result.Success();
    }

    public Result AddImage(ImageEntity image)
    {
        Guard.Against.Null(image, nameof(image));

        if (_images.Any(c => c.Id == image.Id))
        {
            return Result.Error("Image already exists");
        }

        _images.Add(image);
        return Result.Success();
    }

    public Result RemoveImage(ImageEntity image)
    {
        Guard.Against.Null(image, nameof(image));

        if (!_images.Contains(image))
            throw new NotFoundException(nameof(image), "Shopping image not found");

        _images.Remove(image);
        return Result.Success();
    }
}

using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Product.DTOs;
using DemoShop.Domain.Product.Events;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.Product.Entities;

// TODO check for value objects
public class ImageEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    private ImageEntity()
    {
        Name = string.Empty;
        Uri = new Uri("about:blank");
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    private ImageEntity(string name, Uri uri)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        // TODO create invalidUriGuard
        Uri = Guard.Against.InvalidInput(uri, nameof(uri),
            u => Uri.TryCreate(u.ToString(), UriKind.Absolute, out var _),
            "URI must be a valid absolute URI.");
        Audit = Audit.Create();
        SoftDelete = SoftDelete.Create();
    }

    public int Id { get; }
    public string Name { get; private set; }
    public Uri Uri { get; private set; }
    public int? ProductId { get; private set; }
    public ProductEntity? Product { get; private set; }
    public Audit Audit { get; }
    public SoftDelete SoftDelete { get; }

    public static Result<ImageEntity> Create(CreateImageDto createImage)
    {
        Guard.Against.Null(createImage, nameof(createImage));
        Guard.Against.NullOrWhiteSpace(createImage.Name, nameof(createImage.Name));
        Guard.Against.Null(createImage.Uri, nameof(createImage.Uri));

        var image = new ImageEntity(createImage.Name, createImage.Uri);
        image.AddDomainEvent(new ImageCreatedDomainEvent(image));
        return Result.Success(image);
    }

    public Result Delete()
    {
        if (SoftDelete.Deleted)
            return Result.Error("Image is already deleted");

        SoftDelete.MarkAsDeleted();
        this.AddDomainEvent(new ImageDeletedDomainEvent(Id));

        return Result.Success();
    }

    public Result Restore()
    {
        if (!SoftDelete.Deleted)
            return Result.Error("Image is not deleted");

        SoftDelete.MarkAsDeleted();
        this.AddDomainEvent(new ImageRestoredDomainEvent(Id));

        return Result.Success();
    }
}

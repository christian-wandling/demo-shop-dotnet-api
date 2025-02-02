using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Exceptions;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.ValueObjects;
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
            u => Uri.TryCreate(u.ToString(), UriKind.Absolute, out _),
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

    public void Delete()
    {
        if (SoftDelete.Deleted)
            throw new AlreadyMarkedAsDeletedException($"Image {Id} has already been marked as deleted");

        SoftDelete.MarkAsDeleted();
        Audit.UpdateModified();
        this.AddDomainEvent(new ImageDeletedDomainEvent(Id));
    }

    public void Restore()
    {
        if (SoftDelete.Deleted) throw new NotMarkedAsDeletedException($"Image {Id} has not been marked as deleted");

        SoftDelete.Restore();
        Audit.UpdateModified();
        this.AddDomainEvent(new ImageRestoredDomainEvent(Id));
    }
}

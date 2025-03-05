#region

using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Product.Entities;

#endregion

namespace DemoShop.Domain.Product.Events;

public record ProductImageCreatedDomainEvent(ImageEntity Image) : DomainEvent, IDomainEvent;

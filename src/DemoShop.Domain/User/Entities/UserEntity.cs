﻿using Ardalis.GuardClauses;
using Ardalis.Result;
using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Events;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Events;
using DemoShop.Domain.User.DTOs;
using DemoShop.Domain.User.Events;
using DemoShop.Domain.User.ValueObjects;

namespace DemoShop.Domain.User.Entities;

public sealed class UserEntity : IEntity, IAuditable, ISoftDeletable, IAggregateRoot
{
    private readonly List<ShoppingSessionEntity> _shoppingSessions = [];
    private readonly List<OrderEntity> _orders = [];

    private UserEntity()
    {
        KeycloakUserId = KeycloakUserId.Empty;
        Email = EmailAddress.Empty;
        PersonName = PersonName.Empty;
        Audit = Audit.Create();
        SoftDeleteAudit = SoftDeleteAudit.Create();
    }

    private UserEntity(IUserIdentity userIdentity)
    {
        KeycloakUserId = KeycloakUserId.Create(userIdentity.KeycloakId);
        Email = EmailAddress.Create(userIdentity.Email);
        PersonName = PersonName.Create(userIdentity.FirstName, userIdentity.LastName);
        Audit = Audit.Create();
        SoftDeleteAudit = SoftDeleteAudit.Create();
    }

    public int Id { get; }
    public KeycloakUserId KeycloakUserId { get; private init; }
    public EmailAddress Email { get; private init; }
    public PersonName PersonName { get; private set; }
    public PhoneNumber? Phone { get; private set; }
    public AddressEntity? Address { get; private set; }
    public Audit Audit { get; private set; }
    public SoftDeleteAudit SoftDeleteAudit { get; private set; }
    public IReadOnlyCollection<OrderEntity> Orders => _orders.AsReadOnly();
    public IReadOnlyCollection<ShoppingSessionEntity> ShoppingSessions => _shoppingSessions.AsReadOnly();

    public static Result<UserEntity> Create(IUserIdentity userIdentity)
    {
        Guard.Against.Null(userIdentity, nameof(userIdentity));

        var user = new UserEntity(userIdentity);
        user.AddDomainEvent(new UserCreatedDomainEvent(user));
        return Result.Success(user);
    }

    public Result MarkAsDeleted()
    {
        if (SoftDeleteAudit.IsDeleted)
            return Result.Error("User is already deleted");

        SoftDeleteAudit.MarkAsDeleted();
        this.AddDomainEvent(new UserDeletedDomainEvent(this.Id));

        return Result.Success();
    }

    public Result<string> UpdatePhone(string phone)
    {
        Guard.Against.NullOrWhiteSpace(phone, nameof(phone));

        var oldPhone = Phone;
        Phone = PhoneNumber.Create(phone);
        Audit.UpdateModified();

        this.AddDomainEvent(new UserPhoneUpdatedDomainEvent(Id, Phone, oldPhone));

        return Result.Success();
    }

    public Result AddShoppingSession(ShoppingSessionEntity session)
    {
        Guard.Against.Null(session, nameof(session));

        if (_shoppingSessions.Any(s => s.Id == session.Id))
        {
            return Result.Error("Shopping session already exists");
        }

        _shoppingSessions.Add(session);
        this.AddDomainEvent(new ShoppingSessionAddedDomainEvent(Id, session.Id));
        return Result.Success();
    }

    public Result RemoveShoppingSession(ShoppingSessionEntity session)
    {
        Guard.Against.Null(session, nameof(session));

        if (!_shoppingSessions.Contains(session))
            throw new NotFoundException(nameof(session), "Shopping session not found");

        _shoppingSessions.Remove(session);
        this.AddDomainEvent(new OrderRemovedDomainEvent(Id, session.Id));
        return Result.Success();
    }

    public Result AddOrder(OrderEntity order)
    {
        Guard.Against.Null(order, nameof(order));

        if (_orders.Any(o => o.Id == order.Id))
        {
            return Result.Error("Order already exists");
        }

        _orders.Add(order);
        this.AddDomainEvent(new OrderAddedDomainEvent(Id, order.Id));
        return Result.Success();
    }

    public Result RemoveShoppingSession(OrderEntity order)
    {
        Guard.Against.Null(order, nameof(order));

        if (!_orders.Contains(order))
            throw new NotFoundException(nameof(order), "Order not found");

        _orders.Remove(order);
        this.AddDomainEvent(new OrderRemovedDomainEvent(Id, order.Id));
        return Result.Success();
    }

    public Result<AddressEntity> SetInitialAddress(CreateAddressDto createAddress)
    {
        if (Address != null)
            return Result.Error("An address already exists. Use UpdateAddress to modify the existing address.");

        var addressResult = AddressEntity.Create(createAddress);
        if (addressResult.IsError())
            return addressResult;

        Address = addressResult.Value;
        this.AddDomainEvent(new UserAddressUpdatedDomainEvent(Id, Address, null));
        return Result.Success(Address);
    }

    public Result<AddressEntity> UpdateAddress(UpdateAddressDto updateAddress)
    {
        if (Address == null)
            return Result.Error("No existing address to update. Use AddAddress to add an address.");

        var oldAddress = Address;
        var result = Address.Update(updateAddress);
        this.AddDomainEvent(new UserAddressUpdatedDomainEvent(Id, Address, oldAddress));

        return result;
    }
}

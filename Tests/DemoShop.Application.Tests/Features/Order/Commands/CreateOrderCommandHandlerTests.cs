#region

using System.Reflection;
using Ardalis.Result;
using DemoShop.Application.Features.Order.Commands.CreateOrder;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Interfaces;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.TestUtils.Common.Base;
using Microsoft.EntityFrameworkCore;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Application.Tests.Features.Order.Commands;

public class CreateOrderCommandHandlerTests : Test
{
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly IOrderRepository _repository;
    private readonly CreateOrderCommandHandler _sut;

    public CreateOrderCommandHandlerTests()
    {
        _repository = Mock<IOrderRepository>();
        _eventDispatcher = Mock<IDomainEventDispatcher>();
        var logger = Mock<ILogger>();
        _sut = new CreateOrderCommandHandler(_repository, _eventDispatcher, logger);
    }

    [Fact]
    public async Task Handle_WhenSessionConversionSucceeds_ShouldReturnSuccessResult()
    {
        // Arrange
        var product = Create<ProductEntity>();
        var backingField = typeof(ProductEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(product, 1);

        var session = Create<ShoppingSessionEntity>();
        backingField = typeof(ShoppingSessionEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(session, 1);

        session.AddCartItem(product.Id);
        var cartItem = session.CartItems.FirstOrDefault();
        typeof(CartItemEntity)
            .GetProperty(nameof(CartItemEntity.Product))!
            .SetValue(cartItem, product);

        var command = new CreateOrderCommand(session);
        var unsavedOrder = session.ConvertToOrder().Value;

        _repository.CreateOrderAsync(Arg.Any<OrderEntity>(), Arg.Any<CancellationToken>()).Returns(unsavedOrder);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(unsavedOrder);

        await _repository.Received(1).CreateOrderAsync(Arg.Any<OrderEntity>(), Arg.Any<CancellationToken>());
        await _eventDispatcher.Received(1).DispatchEventsAsync(Arg.Any<OrderEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSessionConversionFails_ShouldReturnError()
    {
        // Arrange
        var command = Create<CreateOrderCommand>();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);

        await _repository.DidNotReceive().CreateOrderAsync(Arg.Any<OrderEntity>(), Arg.Any<CancellationToken>());
        await _eventDispatcher.DidNotReceive()
            .DispatchEventsAsync(Arg.Any<OrderEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOrderCreationFails_ShouldReturnError()
    {
        // Arrange
        var product = Create<ProductEntity>();
        var backingField = typeof(ProductEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(product, 1);

        var session = Create<ShoppingSessionEntity>();
        backingField = typeof(ShoppingSessionEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(session, 1);

        session.AddCartItem(product.Id);
        var cartItem = session.CartItems.FirstOrDefault();
        typeof(CartItemEntity)
            .GetProperty(nameof(CartItemEntity.Product))!
            .SetValue(cartItem, product);

        var command = new CreateOrderCommand(session);

        _repository.CreateOrderAsync(Arg.Any<OrderEntity>(), Arg.Any<CancellationToken>()).Returns((OrderEntity?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);

        await _repository.Received(1).CreateOrderAsync(Arg.Any<OrderEntity>(), Arg.Any<CancellationToken>());
        await _eventDispatcher.DidNotReceive()
            .DispatchEventsAsync(Arg.Any<OrderEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ShouldReturnError()
    {
        // Arrange
        var product = Create<ProductEntity>();
        var backingField = typeof(ProductEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(product, 1);

        var session = Create<ShoppingSessionEntity>();
        backingField = typeof(ShoppingSessionEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(session, 1);

        session.AddCartItem(product.Id);
        var cartItem = session.CartItems.FirstOrDefault();
        typeof(CartItemEntity)
            .GetProperty(nameof(CartItemEntity.Product))!
            .SetValue(cartItem, product);

        var command = new CreateOrderCommand(session);

        var exception = new InvalidOperationException("Message");

        _repository.CreateOrderAsync(Arg.Any<OrderEntity>(), Arg.Any<CancellationToken>())
            .Throws(exception);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenDbUpdateExceptionOccurs_ShouldReturnError()
    {
        // Arrange
        var product = Create<ProductEntity>();
        var backingField = typeof(ProductEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(product, 1);

        var session = Create<ShoppingSessionEntity>();
        backingField = typeof(ShoppingSessionEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(session, 1);

        session.AddCartItem(product.Id);
        var cartItem = session.CartItems.FirstOrDefault();
        typeof(CartItemEntity)
            .GetProperty(nameof(CartItemEntity.Product))!
            .SetValue(cartItem, product);

        var command = new CreateOrderCommand(session);

        _repository.CreateOrderAsync(Arg.Any<OrderEntity>(), Arg.Any<CancellationToken>())
            .Throws(new DbUpdateException("Database error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Theory]
    [InlineData(null)]
    public async Task Handle_WhenRequestIsNull_ShouldThrowArgumentNullException(
        CreateOrderCommand request)
    {
        // Act
        var act = () => _sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(request));
    }
}

#region

using System.Reflection;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.Commands.AddCartItem;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.TestUtils.Common.Base;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Application.Tests.Features.ShoppingSession.Commands;

public class AddCartItemCommandHandlerTests : Test
{
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly IMapper _mapper;
    private readonly IShoppingSessionRepository _repository;
    private readonly ICurrentShoppingSessionAccessor _sessionAccessor;
    private readonly AddCartItemCommandHandler _sut;
    private readonly IValidationService _validationService;
    private readonly IValidator<AddCartItemCommand> _validator;

    public AddCartItemCommandHandlerTests()
    {
        _mapper = Substitute.For<IMapper>();
        _sessionAccessor = Mock<ICurrentShoppingSessionAccessor>();
        _repository = Mock<IShoppingSessionRepository>();
        var logger = Mock<ILogger>();
        _eventDispatcher = Mock<IDomainEventDispatcher>();
        _validator = Mock<IValidator<AddCartItemCommand>>();
        _validationService = Mock<IValidationService>();

        _sut = new AddCartItemCommandHandler(
            _mapper,
            _sessionAccessor,
            _repository,
            logger,
            _eventDispatcher,
            _validator,
            _validationService
        );
    }

    [Fact]
    public async Task Handle_WhenRequestIsNull_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.Handle(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ReturnsValidationError()
    {
        // Arrange
        var command = Create<AddCartItemCommand>();
        var validationResult = Result.Invalid();

        _validationService
            .ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(validationResult);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_WhenSessionAccessorFails_ReturnsError()
    {
        // Arrange
        var command = Create<AddCartItemCommand>();
        _validationService
            .ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Task.FromResult(Result.Success()));

        _sessionAccessor
            .GetCurrent(CancellationToken.None)
            .Returns(Result.Error("Session not found"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenAddCartItemSucceeds_ReturnsCartItemResponse()
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

        var command = new AddCartItemCommand(
            new AddCartItemRequest { ProductId = product.Id }
        );
        var cartItem = Create<CartItemEntity>();
        typeof(CartItemEntity)
            .GetProperty(nameof(CartItemEntity.Product))!
            .SetValue(cartItem, product);
        typeof(CartItemEntity)
            .GetProperty(nameof(CartItemEntity.ProductId))!
            .SetValue(cartItem, product.Id);

        var cartItemResponse = new CartItemResponse
        {
            Id = cartItem.Id,
            ProductId = product.Id,
            ProductName = product.Name,
            ProductThumbnail = product.Thumbnail!,
            Quantity = cartItem.Quantity.Value,
            UnitPrice = cartItem.Product!.Price.Value,
            TotalPrice = cartItem.TotalPrice
        };

        _validationService
            .ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Task.FromResult(Result.Success()));

        _sessionAccessor
            .GetCurrent(CancellationToken.None)
            .Returns(Task.FromResult(Result.Success(session)));

        _repository
            .UpdateSessionAsync(session, CancellationToken.None)
            .Returns(Task.FromResult(session));

        _mapper
            .Map<CartItemResponse>(Arg.Any<CartItemEntity>())
            .Returns(cartItemResponse);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(cartItemResponse);

        await _eventDispatcher
            .Received(1)
            .DispatchEventsAsync(session, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenDbUpdateExceptionOccurs_ReturnsError()
    {
        // Arrange
        var command = Create<AddCartItemCommand>();
        var session = Create<ShoppingSessionEntity>();
        var backingField = typeof(ShoppingSessionEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(session, 1);

        var exception = new DbUpdateException("Database error");

        _validationService
            .ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Task.FromResult(Result.Success()));

        _sessionAccessor
            .GetCurrent(CancellationToken.None)
            .Returns(Result.Success(session));

        _repository
            .UpdateSessionAsync(session, CancellationToken.None)
            .Throws(exception);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ReturnsError()
    {
        // Arrange
        var command = Create<AddCartItemCommand>();
        var session = Create<ShoppingSessionEntity>();
        var backingField = typeof(ShoppingSessionEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(session, 1);


        var exception = new InvalidOperationException("Invalid operation");

        _validationService
            .ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Task.FromResult(Result.Success()));

        _sessionAccessor
            .GetCurrent(CancellationToken.None)
            .Returns(Result.Success(session));

        session.AddCartItem(command.AddCartItem.ProductId)
            .Throws(exception);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }
}

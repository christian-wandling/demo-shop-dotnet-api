using System.Reflection;
using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.Commands.RemoveCartItem;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.TestUtils.Common.Base;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using DbUpdateException = Microsoft.EntityFrameworkCore.DbUpdateException;

namespace DemoShop.Application.Tests.Features.ShoppingSession.Commands;

public class RemoveCartItemCommandHandlerTests : Test
{
    private readonly RemoveCartItemCommandHandler _sut;
    private readonly ICurrentShoppingSessionAccessor _sessionAccessor;
    private readonly IShoppingSessionRepository _repository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly IValidator<RemoveCartItemCommand> _validator;
    private readonly IValidationService _validationService;

    public RemoveCartItemCommandHandlerTests()
    {
        _sessionAccessor = Mock<ICurrentShoppingSessionAccessor>();
        _repository = Mock<IShoppingSessionRepository>();
        _eventDispatcher = Mock<IDomainEventDispatcher>();
        var logger = Mock<ILogger<RemoveCartItemCommandHandler>>();
        _validator = Mock<IValidator<RemoveCartItemCommand>>();
        _validationService = Mock<IValidationService>();

        _sut = new RemoveCartItemCommandHandler(
            _sessionAccessor,
            _repository,
            _eventDispatcher,
            logger,
            _validator,
            _validationService
        );
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ReturnsValidationError()
    {
        // Arrange
        var command = new RemoveCartItemCommand(1);
        var validationResult = Result.Invalid();

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(validationResult);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_WhenSessionNotFound_ReturnsError()
    {
        // Arrange
        var command = new RemoveCartItemCommand(1);
        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Result.Success());

        _sessionAccessor.GetCurrent(CancellationToken.None)
            .Returns(Result<ShoppingSessionEntity>.Error("Session not found"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenRemoveCartItemNotExists_ReturnsNotFound()
    {
        // Arrange
        var command = new RemoveCartItemCommand(1);
        var session = Create<ShoppingSessionEntity>();

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Result.Success());

        _sessionAccessor.GetCurrent(CancellationToken.None)
            .Returns(Result.Success(session));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        var productId = Create<int>();
        var session = Create<ShoppingSessionEntity>();
        var backingField = typeof(ShoppingSessionEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(session, 1);
        session.AddCartItem(productId);
        var cartItem = session.CartItems.First();
        backingField = typeof(CartItemEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(cartItem, 1);

        var command = new RemoveCartItemCommand(cartItem.Id);

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Result.Success());

        _sessionAccessor.GetCurrent(CancellationToken.None)
            .Returns(Result.Success(session));

        _repository.UpdateSessionAsync(session, CancellationToken.None)
            .Returns(session);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(ResultStatus.NoContent);
        await _eventDispatcher.Received(1).DispatchEventsAsync(session, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenDbUpdateExceptionOccurs_ReturnsError()
    {
        // Arrange
        var productId = Create<int>();
        var session = Create<ShoppingSessionEntity>();
        var backingField = typeof(ShoppingSessionEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(session, 1);
        session.AddCartItem(productId);
        var cartItem = session.CartItems.First();
        backingField = typeof(CartItemEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(cartItem, 1);

        var command = new RemoveCartItemCommand(cartItem.Id);
        var exception = new DbUpdateException("Database error");

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Result.Success());

        _sessionAccessor.GetCurrent(CancellationToken.None)
            .Returns(Result.Success(session));

        _repository.UpdateSessionAsync(session, CancellationToken.None)
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
        var productId = Create<int>();
        var session = Create<ShoppingSessionEntity>();
        var backingField = typeof(ShoppingSessionEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(session, 1);
        session.AddCartItem(productId);

        var cartItem = session.CartItems.First();
        backingField = typeof(CartItemEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(cartItem, 1);


        var command = new RemoveCartItemCommand(cartItem.Id);
        var exception = new InvalidOperationException("Invalid operation");

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Result.Success());

        _sessionAccessor.GetCurrent(CancellationToken.None)
            .Returns(Result.Success(session));

        _repository.UpdateSessionAsync(session, CancellationToken.None)
            .Throws(exception);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }
}

#region

using System.Reflection;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.Commands.UpdateCartItemQuantity;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.TestUtils.Common.Base;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Application.Tests.Features.ShoppingSession.Commands;

[Trait("Feature", "ShoppingSession")]
public class UpdateCartItemQuantityCommandHandlerTests : Test
{
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly IMapper _mapper;
    private readonly IShoppingSessionRepository _repository;
    private readonly ICurrentShoppingSessionAccessor _sessionAccessor;
    private readonly UpdateCartItemQuantityCommandHandler _sut;
    private readonly IValidationService _validationService;
    private readonly IValidator<UpdateCartItemQuantityCommand> _validator;

    public UpdateCartItemQuantityCommandHandlerTests()
    {
        _mapper = Substitute.For<IMapper>();
        _sessionAccessor = Mock<ICurrentShoppingSessionAccessor>();
        _repository = Mock<IShoppingSessionRepository>();
        var logger = Mock<ILogger>();
        _eventDispatcher = Mock<IDomainEventDispatcher>();
        _validator = Mock<IValidator<UpdateCartItemQuantityCommand>>();
        _validationService = Mock<IValidationService>();

        _sut = new UpdateCartItemQuantityCommandHandler(
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
    public async Task Handle_WithValidRequest_ShouldUpdateCartItemQuantitySuccessfully()
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

        var command = new UpdateCartItemQuantityCommand(cartItem.Id,
            new UpdateCartItemQuantityRequest { Quantity = Create<int>() });
        var response = Create<UpdateCartItemQuantityResponse>();

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Task.FromResult(Result.Success()));

        _sessionAccessor.GetCurrent(CancellationToken.None)
            .Returns(Task.FromResult(Result.Success(session)));

        _repository.UpdateSessionAsync(session, CancellationToken.None)
            .Returns(Task.FromResult(session));

        _mapper.Map<UpdateCartItemQuantityResponse>(cartItem)
            .Returns(response);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(response);
        await _eventDispatcher.Received(1).DispatchEventsAsync(session, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureResult()
    {
        // Arrange
        var command = Create<UpdateCartItemQuantityCommand>();
        const string validationError = "Validation failed";

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Task.FromResult(Result.Error(validationError)));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenSessionNotFound_ShouldReturnFailureResult()
    {
        // Arrange
        var command = Create<UpdateCartItemQuantityCommand>();
        const string sessionError = "Session not found";

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Task.FromResult(Result.Success()));

        _sessionAccessor.GetCurrent(CancellationToken.None)
            .Returns(Result.Error(sessionError));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        await _repository.DidNotReceive().UpdateSessionAsync(Arg.Any<ShoppingSessionEntity>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenUpdateCartItemNotFound_ShouldReturnNotFoundResult()
    {
        // Arrange
        var command = Create<UpdateCartItemQuantityCommand>();
        var session = Create<ShoppingSessionEntity>();

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Task.FromResult(Result.Success()));

        _sessionAccessor.GetCurrent(CancellationToken.None)
            .Returns(Task.FromResult(Result.Success(session)));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        await _repository.DidNotReceive().UpdateSessionAsync(Arg.Any<ShoppingSessionEntity>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ShouldReturnFailureResult()
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

        var command = new UpdateCartItemQuantityCommand(cartItem.Id,
            new UpdateCartItemQuantityRequest { Quantity = Create<int>() });
        const string errorMessage = "Invalid operation";

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Task.FromResult(Result.Success()));

        _sessionAccessor.GetCurrent(CancellationToken.None)
            .Returns(Task.FromResult(Result.Success(session)));

        _repository.UpdateSessionAsync(session, CancellationToken.None)
            .Throws(new InvalidOperationException(errorMessage));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenDbUpdateExceptionOccurs_ShouldReturnFailureResult()
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

        var command = new UpdateCartItemQuantityCommand(cartItem.Id,
            new UpdateCartItemQuantityRequest { Quantity = Create<int>() });

        const string errorMessage = "Database update failed";

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Task.FromResult(Result.Success()));

        _sessionAccessor.GetCurrent(CancellationToken.None)
            .Returns(Task.FromResult(Result.Success(session)));

        _repository.UpdateSessionAsync(session, CancellationToken.None)
            .Throws(new DbUpdateException(errorMessage));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_WithInvalidId_ShouldThrowArgumentException(int invalidId)
    {
        // Arrange
        var command = new UpdateCartItemQuantityCommand(
            invalidId,
            new UpdateCartItemQuantityRequest { Quantity = 1 }
        );

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage(
                $"Required input {nameof(command.Id)} cannot be zero or negative. (Parameter '{nameof(command.Id)}')");
    }
}

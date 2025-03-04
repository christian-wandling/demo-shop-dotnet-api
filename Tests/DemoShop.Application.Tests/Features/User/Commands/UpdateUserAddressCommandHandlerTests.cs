#region

using System.Reflection;
using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.Commands.UpdateUserAddress;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using DemoShop.TestUtils.Common.Base;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Application.Tests.Features.User.Commands;

public class UpdateUserAddressCommandHandlerTests : Test
{
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly UpdateUserAddressCommandHandler _handler;
    private readonly IUserIdentityAccessor _identity;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;
    private readonly IValidationService _validationService;
    private readonly IValidator<UpdateUserAddressCommand> _validator;

    public UpdateUserAddressCommandHandlerTests()
    {
        _mapper = Substitute.For<IMapper>();
        _identity = Mock<IUserIdentityAccessor>();
        _repository = Mock<IUserRepository>();
        _logger = Mock<ILogger>();
        _eventDispatcher = Mock<IDomainEventDispatcher>();
        _validator = Mock<IValidator<UpdateUserAddressCommand>>();
        _validationService = Mock<IValidationService>();

        _handler = new UpdateUserAddressCommandHandler(
            _mapper,
            _identity,
            _repository,
            _logger,
            _eventDispatcher,
            _validator,
            _validationService
        );
    }

    [Fact]
    public async Task Handle_WithValidRequestAndExistingAddress_ShouldUpdateAndReturnSuccess()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        var userIdentity = Create<IUserIdentity>();
        var user = Create<UserEntity>();
        var backingField = typeof(UserEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(user, 1);
        var addressResponse = Create<AddressResponse>();

        _validationService.ValidateAsync(command, _validator, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        _identity.GetCurrentIdentity()
            .Returns(Result.Success(userIdentity));

        _repository.GetUserByKeycloakIdAsync(userIdentity.KeycloakUserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _repository.UpdateUserAsync(user, Arg.Any<CancellationToken>())
            .Returns(user);

        _mapper.Map<AddressResponse>(Arg.Any<AddressEntity>())
            .Returns(addressResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(addressResponse);
        await _eventDispatcher.Received(1).DispatchEventsAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidRequestAndNoExistingAddress_ShouldCreateAndReturnSuccess()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        var userIdentity = Create<IUserIdentity>();
        var user = Create<UserEntity>();
        var backingField = typeof(UserEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(user, 1);
        var addressResponse = Create<AddressResponse>();

        _validationService.ValidateAsync(command, _validator, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        _identity.GetCurrentIdentity()
            .Returns(Result.Success(userIdentity));

        _repository.GetUserByKeycloakIdAsync(userIdentity.KeycloakUserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _repository.UpdateUserAsync(user, Arg.Any<CancellationToken>())
            .Returns(user);

        _mapper.Map<AddressResponse>(Arg.Any<AddressEntity>())
            .Returns(addressResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(addressResponse);
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ShouldReturnValidationError()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        var validationError = Result.Invalid();


        _validationService.ValidateAsync(command, _validator, Arg.Any<CancellationToken>())
            .Returns(validationError);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        await _repository.DidNotReceive().UpdateUserAsync(Arg.Any<UserEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        var userIdentity = Create<IUserIdentity>();

        _validationService.ValidateAsync(command, _validator, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        _identity.GetCurrentIdentity()
            .Returns(Result.Success(userIdentity));

        _repository.GetUserByKeycloakIdAsync(userIdentity.KeycloakUserId, Arg.Any<CancellationToken>())
            .Returns((UserEntity)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_WhenIdentityAccessFails_ShouldReturnError()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        var identityError = Result.Error("Identity error");

        _validationService.ValidateAsync(command, _validator, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        _identity.GetCurrentIdentity()
            .Returns(identityError);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenDbUpdateExceptionOccurs_ShouldReturnError()
    {
        // Arrange
        var command = Create<UpdateUserAddressCommand>();
        var userIdentity = Create<IUserIdentity>();
        var user = Create<UserEntity>();
        var backingField = typeof(UserEntity)
            .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        backingField?.SetValue(user, Create<int>());

        var exception = new DbUpdateException("Database error");

        _validationService.ValidateAsync(command, _validator, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        _identity.GetCurrentIdentity()
            .Returns(Result.Success(userIdentity));

        _repository.GetUserByKeycloakIdAsync(userIdentity.KeycloakUserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _repository.UpdateUserAsync(user, Arg.Any<CancellationToken>())
            .Throws(exception);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Theory]
    [InlineData(null)]
    public async Task Handle_WithNullRequest_ShouldThrowArgumentNullException(UpdateUserAddressCommand request)
    {
        // Act
        var act = () => _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}

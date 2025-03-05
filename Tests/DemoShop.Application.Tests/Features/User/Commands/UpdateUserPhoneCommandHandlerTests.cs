#region

using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.Commands.UpdateUserPhone;
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

public class UpdateUserPhoneCommandHandlerTests : Test
{
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly IUserIdentityAccessor _identity;
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;
    private readonly UpdateUserPhoneCommandHandler _sut;
    private readonly IValidationService _validationService;
    private readonly IValidator<UpdateUserPhoneCommand> _validator;

    public UpdateUserPhoneCommandHandlerTests()
    {
        _mapper = Substitute.For<IMapper>();
        _identity = Mock<IUserIdentityAccessor>();
        _repository = Mock<IUserRepository>();
        var logger = Mock<ILogger>();
        _eventDispatcher = Mock<IDomainEventDispatcher>();
        _validator = Mock<IValidator<UpdateUserPhoneCommand>>();
        _validationService = Mock<IValidationService>();

        _sut = new UpdateUserPhoneCommandHandler(
            _mapper,
            _identity,
            _repository,
            logger,
            _eventDispatcher,
            _validator,
            _validationService
        );
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ReturnsValidationError()
    {
        // Arrange
        var command = Create<UpdateUserPhoneCommand>();
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
    public async Task Handle_WhenIdentityAccessFails_ReturnsIdentityError()
    {
        // Arrange
        var command = Create<UpdateUserPhoneCommand>();
        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Result.Success());

        _identity.GetCurrentIdentity()
            .Returns(Result.Error("Identity error"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = Create<UpdateUserPhoneCommand>();
        var identity = Create<IUserIdentity>();

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Result.Success());
        _identity.GetCurrentIdentity()
            .Returns(Result.Success(identity));
        _repository.GetUserByKeycloakIdAsync(identity.KeycloakUserId, CancellationToken.None)
            .Returns((UserEntity)null!);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ReturnsUserPhoneResponse()
    {
        // Arrange
        var command = Create<UpdateUserPhoneCommand>();
        var identity = Create<IUserIdentity>();
        var user = Create<UserEntity>();
        var updatedUser = Create<UserEntity>();
        var response = Create<UserPhoneResponse>();

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Result.Success());
        _identity.GetCurrentIdentity()
            .Returns(Result.Success(identity));
        _repository.GetUserByKeycloakIdAsync(identity.KeycloakUserId, CancellationToken.None)
            .Returns(user);
        _repository.UpdateUserAsync(user, CancellationToken.None)
            .Returns(updatedUser);
        _mapper.Map<UserPhoneResponse>(Arg.Any<UserEntity>())
            .Returns(response);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(response);
        await _eventDispatcher.Received(1).DispatchEventsAsync(user, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ReturnsError()
    {
        // Arrange
        var command = Create<UpdateUserPhoneCommand>();
        var identity = Create<IUserIdentity>();
        var exception = new InvalidOperationException("Invalid operation");

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Result.Success());
        _identity.GetCurrentIdentity()
            .Returns(Result.Success(identity));
        _repository.GetUserByKeycloakIdAsync(identity.KeycloakUserId, CancellationToken.None)
            .Throws(exception);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenDbUpdateExceptionOccurs_ReturnsError()
    {
        // Arrange
        var command = Create<UpdateUserPhoneCommand>();
        var identity = Create<IUserIdentity>();
        var user = Create<UserEntity>();
        var exception = new DbUpdateException("Database error");

        _validationService.ValidateAsync(command, _validator, CancellationToken.None)
            .Returns(Result.Success());
        _identity.GetCurrentIdentity()
            .Returns(Result.Success(identity));
        _repository.GetUserByKeycloakIdAsync(identity.KeycloakUserId, CancellationToken.None)
            .Returns(user);
        _repository.UpdateUserAsync(user, CancellationToken.None)
            .Throws(exception);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }
}

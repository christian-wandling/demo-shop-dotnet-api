using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using DemoShop.TestUtils.Common.Base;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

namespace DemoShop.Application.Tests.Features.User.Commands;

public class CreateUserCommandHandlerTests : Test
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly IValidationService _validationService;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _mapper = Mock<IMapper>();
        _repository = Mock<IUserRepository>();
        _logger = Mock<ILogger<CreateUserCommandHandler>>();
        _eventDispatcher = Mock<IDomainEventDispatcher>();
        _validator = Mock<IValidator<CreateUserCommand>>();
        _validationService = Mock<IValidationService>();

        _handler = new CreateUserCommandHandler(
            _mapper,
            _repository,
            _logger,
            _eventDispatcher,
            _validator,
            _validationService
        );
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var command = Create<CreateUserCommand>();
        var userEntity = Create<UserEntity>();
        var userResponse = Create<UserResponse>();

        _validationService.ValidateAsync(command, _validator, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        _repository.CreateUserAsync(Arg.Any<UserEntity>(), Arg.Any<CancellationToken>())
            .Returns(userEntity);

        _mapper.Map<UserResponse>(userEntity)
            .Returns(userResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(userResponse);
        await _eventDispatcher.Received(1).DispatchEventsAsync(Arg.Any<UserEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ShouldReturnValidationError()
    {
        // Arrange
        var command = Create<CreateUserCommand>();
        var validationError = Result.Invalid();

        _validationService.ValidateAsync(command, _validator, Arg.Any<CancellationToken>())
            .Returns(validationError);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        await _repository.DidNotReceive().CreateUserAsync(Arg.Any<UserEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRepositoryCreateFails_ShouldReturnError()
    {
        // Arrange
        var command = Create<CreateUserCommand>();

        _validationService.ValidateAsync(command, _validator, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        _repository.CreateUserAsync(Arg.Any<UserEntity>(), Arg.Any<CancellationToken>())
            .Returns((UserEntity)null!);

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
        var command = Create<CreateUserCommand>();
        var exception = new DbUpdateException("Database error");

        _validationService.ValidateAsync(command, _validator, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        _repository.CreateUserAsync(Arg.Any<UserEntity>(), Arg.Any<CancellationToken>())
            .Throws(exception);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        _logger.Received(1).LogOperationFailed(
            "Create user",
            "KeycloakUserId",
            command.UserIdentity.KeycloakUserId,
            null);
    }

    [Theory]
    [InlineData(null)]
    public async Task Handle_WithNullRequest_ShouldThrowArgumentNullException(CreateUserCommand request)
    {
        // Act
        var act = () => _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}


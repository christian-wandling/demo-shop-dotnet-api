using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.Commands.CreateShoppingSession;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.TestUtils.Common.Base;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using DbUpdateException = Microsoft.EntityFrameworkCore.DbUpdateException;

namespace DemoShop.Application.Tests.Features.ShoppingSession.Commands;

public class CreateShoppingSessionCommandHandlerTests : Test
{
    private readonly CreateShoppingSessionCommandHandler _sut;
    private readonly IMapper _mapper;
    private readonly IShoppingSessionRepository _repository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly IValidator<CreateShoppingSessionCommand> _validator;
    private readonly IValidationService _validationService;

    public CreateShoppingSessionCommandHandlerTests()
    {
        _mapper = Substitute.For<IMapper>();
        _repository = Mock<IShoppingSessionRepository>();
        var logger = Mock<ILogger<CreateShoppingSessionCommandHandler>>();
        _eventDispatcher = Mock<IDomainEventDispatcher>();
        _validator = Mock<IValidator<CreateShoppingSessionCommand>>();
        _validationService = Mock<IValidationService>();

        _sut = new CreateShoppingSessionCommandHandler(
            _mapper,
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
        // Arrange
        CreateShoppingSessionCommand? request = null;

        // Act
        var act = () => _sut.Handle(request!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_WhenUserIdIsInvalid_ThrowsArgumentException(int userId)
    {
        // Arrange
        var request = new CreateShoppingSessionCommand(userId);

        // Act
        var act = () => _sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ReturnsValidationError()
    {
        // Arrange
        var request = new CreateShoppingSessionCommand(1);
        var validationError = Result.Invalid();


        _validationService.ValidateAsync(request, _validator, CancellationToken.None)
            .Returns(validationError);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ReturnsShoppingSessionResponse()
    {
        // Arrange
        var request = new CreateShoppingSessionCommand(1);
        var validationSuccess = Result.Success();
        var shoppingSession = Create<ShoppingSessionEntity>();
        var response = Create<ShoppingSessionResponse>();

        _validationService.ValidateAsync(request, _validator, CancellationToken.None)
            .Returns(validationSuccess);
        _repository.CreateSessionAsync(Arg.Any<ShoppingSessionEntity>(), CancellationToken.None)
            .Returns(shoppingSession);
        _mapper.Map<ShoppingSessionResponse>(shoppingSession)
            .Returns(response);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(response);
        await _eventDispatcher.Received(1).DispatchEventsAsync(Arg.Any<ShoppingSessionEntity>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNull_ReturnsError()
    {
        // Arrange
        var request = new CreateShoppingSessionCommand(1);
        var validationSuccess = Result.Success();

        _validationService.ValidateAsync(request, _validator, CancellationToken.None)
            .Returns(validationSuccess);
        _repository.CreateSessionAsync(Arg.Any<ShoppingSessionEntity>(), CancellationToken.None)
            .Returns((ShoppingSessionEntity?)null);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenDbUpdateExceptionOccurs_ReturnsError()
    {
        // Arrange
        var request = new CreateShoppingSessionCommand(1);
        var validationSuccess = Result.Success();
        var exception = new DbUpdateException("Database error");

        _validationService.ValidateAsync(request, _validator, CancellationToken.None)
            .Returns(validationSuccess);
        _repository.CreateSessionAsync(Arg.Any<ShoppingSessionEntity>(), CancellationToken.None)
            .Throws(exception);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ReturnsError()
    {
        // Arrange
        var request = new CreateShoppingSessionCommand(1);
        var validationSuccess = Result.Success();
        var exception = new InvalidOperationException("Invalid operation");

        _validationService.ValidateAsync(request, _validator, CancellationToken.None)
            .Returns(validationSuccess);
        _repository.CreateSessionAsync(Arg.Any<ShoppingSessionEntity>(), CancellationToken.None)
            .Throws(exception);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }
}


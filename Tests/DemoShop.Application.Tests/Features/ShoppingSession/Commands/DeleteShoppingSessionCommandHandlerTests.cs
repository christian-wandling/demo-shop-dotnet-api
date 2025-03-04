#region

using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.Commands.DeleteShoppingSession;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.TestUtils.Common.Base;
using Serilog;
using NSubstitute.ExceptionExtensions;
using DbUpdateException = Microsoft.EntityFrameworkCore.DbUpdateException;

#endregion

namespace DemoShop.Application.Tests.Features.ShoppingSession.Commands;

public class DeleteShoppingSessionCommandHandlerTests : Test
{
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger _logger;
    private readonly IShoppingSessionRepository _repository;
    private readonly DeleteShoppingSessionCommandHandler _sut;

    public DeleteShoppingSessionCommandHandlerTests()
    {
        _repository = Mock<IShoppingSessionRepository>();
        _eventDispatcher = Mock<IDomainEventDispatcher>();
        _logger = Mock<ILogger>();
        _sut = new DeleteShoppingSessionCommandHandler(_repository, _eventDispatcher, _logger);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnNoContent()
    {
        // Arrange
        var session = Create<ShoppingSessionEntity>();
        var command = new DeleteShoppingSessionCommand(session);
        _repository.DeleteSessionAsync(session, CancellationToken.None).Returns(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Result>();
        result.IsSuccess.Should().BeTrue();

        await _repository.Received(1).DeleteSessionAsync(session, CancellationToken.None);
        await _eventDispatcher.Received(1).DispatchEventsAsync(session, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenDeleteFails_ShouldReturnError()
    {
        // Arrange
        var session = Create<ShoppingSessionEntity>();
        var command = new DeleteShoppingSessionCommand(session);
        _repository.DeleteSessionAsync(session, CancellationToken.None).Returns(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Result>();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);

        await _repository.Received(1).DeleteSessionAsync(session, CancellationToken.None);
        await _eventDispatcher.DidNotReceive().DispatchEventsAsync(session, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ShouldLogAndReturnError()
    {
        // Arrange
        var session = Create<ShoppingSessionEntity>();
        var command = new DeleteShoppingSessionCommand(session);
        var exception = new InvalidOperationException("Invalid operation");
        _repository.DeleteSessionAsync(session, CancellationToken.None).Throws(exception);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Result>();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);

        _logger.Received(1).Error(Arg.Any<Exception>(), Arg.Any<string>());
        await _eventDispatcher.DidNotReceive().DispatchEventsAsync(session, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenDbUpdateExceptionOccurs_ShouldLogAndReturnError()
    {
        // Arrange
        var session = Create<ShoppingSessionEntity>();
        var command = new DeleteShoppingSessionCommand(session);
        var exception = new DbUpdateException("Database error");
        _repository.DeleteSessionAsync(session, CancellationToken.None).Throws(exception);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Result>();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);

        _logger.Received(1).Error(Arg.Any<Exception>(), Arg.Any<string>());
        await _eventDispatcher.DidNotReceive().DispatchEventsAsync(session, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => _sut.Handle(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");
    }

    [Fact]
    public async Task Handle_WithNullRequestSession_ShouldThrowArgumentNullException()
    {
        // Arrange
        var request = new DeleteShoppingSessionCommand(null!);

        // Act
        var act = () => _sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("Session");
    }
}

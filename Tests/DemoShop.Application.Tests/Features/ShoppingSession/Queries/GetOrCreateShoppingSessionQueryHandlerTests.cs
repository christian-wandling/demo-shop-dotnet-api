#region

using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.Commands.CreateShoppingSession;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Queries.GetOrCreateShoppingSession;
using DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

#endregion

namespace DemoShop.Application.Tests.Features.ShoppingSession.Queries;

public class GetOrCreateShoppingSessionQueryHandlerTests : Test
{
    private readonly CancellationToken _cancellationToken;
    private readonly ILogger<GetOrCreateShoppingSessionQueryHandler> _logger;
    private readonly IMediator _mediator;
    private readonly GetOrCreateShoppingSessionQueryHandler _sut;
    private readonly ICurrentUserAccessor _userAccessor;

    public GetOrCreateShoppingSessionQueryHandlerTests()
    {
        _userAccessor = Substitute.For<ICurrentUserAccessor>();
        _mediator = Substitute.For<IMediator>();
        _logger = Substitute.For<ILogger<GetOrCreateShoppingSessionQueryHandler>>();
        _cancellationToken = CancellationToken.None;

        _sut = new GetOrCreateShoppingSessionQueryHandler(
            _userAccessor,
            _mediator,
            _logger
        );
    }

    [Fact]
    public async Task Handle_WhenUserIdNotFound_ReturnsForbidden()
    {
        // Arrange
        _userAccessor.GetId(_cancellationToken)
            .Returns(Result.Forbidden("Unauthorized"));

        // Act
        var result = await _sut.Handle(new GetOrCreateShoppingSessionQuery(), _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Forbidden);
    }

    [Fact]
    public async Task Handle_WhenSessionExists_ReturnsExistingSession()
    {
        // Arrange
        var userId = Create<int>();
        var expectedSession = Create<ShoppingSessionResponse>();

        _userAccessor.GetId(_cancellationToken)
            .Returns(Result.Success(userId));
        _mediator.Send(Arg.Is<GetShoppingSessionByUserIdQuery>(q => q.UserId == userId), _cancellationToken)
            .Returns(Result.Success(expectedSession));

        // Act
        var result = await _sut.Handle(new GetOrCreateShoppingSessionQuery(), _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedSession);
    }

    [Fact]
    public async Task Handle_WhenSessionDoesNotExist_CreatesAndReturnsNewSession()
    {
        // Arrange
        var userId = Create<int>();
        var newSession = Create<ShoppingSessionResponse>();

        _userAccessor.GetId(_cancellationToken)
            .Returns(Result.Success(userId));
        _mediator.Send(Arg.Is<GetShoppingSessionByUserIdQuery>(q => q.UserId == userId), _cancellationToken)
            .Returns(Result.Error("Not found"));
        _mediator.Send(Arg.Is<CreateShoppingSessionCommand>(c => c.UserId == userId), _cancellationToken)
            .Returns(Result.Success(newSession));

        // Act
        var result = await _sut.Handle(new GetOrCreateShoppingSessionQuery(), _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(newSession);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ReturnsError()
    {
        // Arrange
        var userId = Create<int>();
        const string errorMessage = "Invalid operation occurred";

        _userAccessor.GetId(_cancellationToken)
            .Returns(Result.Success(userId));
        _mediator.Send(Arg.Any<GetShoppingSessionByUserIdQuery>(), _cancellationToken)
            .Throws(new InvalidOperationException(errorMessage));

        // Act
        var result = await _sut.Handle(new GetOrCreateShoppingSessionQuery(), _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        _logger.Received(1).LogDomainException(errorMessage);
    }

    [Fact]
    public async Task Handle_WhenDbExceptionOccurs_ReturnsError()
    {
        // Arrange
        var userId = Create<int>();
        const string errorMessage = "Database error occurred";

        _userAccessor.GetId(_cancellationToken)
            .Returns(Result.Success(userId));
        _mediator.Send(Arg.Any<GetShoppingSessionByUserIdQuery>(), _cancellationToken)
            .Throws(new TestDbException(errorMessage));

        // Act
        var result = await _sut.Handle(new GetOrCreateShoppingSessionQuery(), _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        _logger.Received(1).LogOperationFailed(
            "Get Session By UserId",
            "UserId",
            userId.ToString(),
            null);
    }
}

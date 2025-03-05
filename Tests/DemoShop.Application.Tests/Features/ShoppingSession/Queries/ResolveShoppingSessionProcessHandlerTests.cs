#region

using Ardalis.Result;
using DemoShop.Application.Features.ShoppingSession.Commands.CreateShoppingSession;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Processes.ResolveShoppingSession;
using DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using Serilog;

#endregion

namespace DemoShop.Application.Tests.Features.ShoppingSession.Queries;

public class ResolveShoppingSessionProcessHandlerTests : Test
{
    private readonly CancellationToken _cancellationToken;
    private readonly IMediator _mediator;
    private readonly ResolveShoppingSessionProcessHandler _sut;
    private readonly ICurrentUserAccessor _userAccessor;

    public ResolveShoppingSessionProcessHandlerTests()
    {
        _userAccessor = Substitute.For<ICurrentUserAccessor>();
        _mediator = Substitute.For<IMediator>();
        var logger = Substitute.For<ILogger>();
        _cancellationToken = CancellationToken.None;

        _sut = new ResolveShoppingSessionProcessHandler(
            _userAccessor,
            _mediator,
            logger
        );
    }

    [Fact]
    public async Task Handle_WhenUserIdNotFound_ReturnsForbidden()
    {
        // Arrange
        _userAccessor.GetId(_cancellationToken)
            .Returns(Result.Forbidden("Unauthorized"));

        // Act
        var result = await _sut.Handle(new ResolveShoppingSessionProcess(), _cancellationToken);

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
        var result = await _sut.Handle(new ResolveShoppingSessionProcess(), _cancellationToken);

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
        var result = await _sut.Handle(new ResolveShoppingSessionProcess(), _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(newSession);
    }
}

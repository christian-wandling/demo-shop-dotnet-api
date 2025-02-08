#region

using Ardalis.Result;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.Commands.CreateUser;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Queries.GetOrCreateUser;
using DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

#endregion

namespace DemoShop.Application.Tests.Features.User.Queries;

public class GetOrCreateUserQueryHandlerTests : Test
{
    private readonly IUserIdentityAccessor _identity;
    private readonly IMediator _mediator;
    private readonly GetOrCreateUserQueryHandler _sut;

    public GetOrCreateUserQueryHandlerTests()
    {
        _identity = Mock<IUserIdentityAccessor>();
        _mediator = Mock<IMediator>();
        var logger = Mock<ILogger<GetOrCreateUserQueryHandler>>();

        _sut = new GetOrCreateUserQueryHandler(_identity, _mediator, logger);
    }

    [Fact]
    public async Task Handle_WhenIdentityResultFails_ShouldReturnError()
    {
        // Arrange
        var query = Create<GetOrCreateUserQuery>();
        _identity.GetCurrentIdentity().Returns(Result.Error());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldReturnExistingUser()
    {
        // Arrange
        var query = Create<GetOrCreateUserQuery>();
        var userIdentity = Create<IUserIdentity>();
        var userResponse = Create<UserResponse>();

        _identity.GetCurrentIdentity().Returns(Result.Success(userIdentity));
        _mediator.Send(Arg.Any<GetUserByKeycloakIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(userResponse));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(userResponse);
        await _mediator.DidNotReceive().Send(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldCreateAndReturnNewUser()
    {
        // Arrange
        var query = Create<GetOrCreateUserQuery>();
        var userIdentity = Create<IUserIdentity>();
        var userResponse = Create<UserResponse>();

        _identity.GetCurrentIdentity().Returns(Result.Success(userIdentity));
        _mediator.Send(Arg.Any<GetUserByKeycloakIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("User not found"));
        _mediator.Send(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(userResponse));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(userResponse);
        await _mediator.Received(1).Send(Arg.Is<CreateUserCommand>(cmd => cmd.UserIdentity == userIdentity),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ShouldReturnError()
    {
        // Arrange
        var query = Create<GetOrCreateUserQuery>();
        var userIdentity = Create<IUserIdentity>();

        _identity.GetCurrentIdentity().Returns(Result.Success(userIdentity));
        _mediator.Send(Arg.Any<GetUserByKeycloakIdQuery>(), Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenDbExceptionOccurs_ShouldReturnError()
    {
        // Arrange
        var query = Create<GetOrCreateUserQuery>();
        var userIdentity = Create<IUserIdentity>();

        _identity.GetCurrentIdentity().Returns(Result.Success(userIdentity));
        _mediator.Send(Arg.Any<GetUserByKeycloakIdQuery>(), Arg.Any<CancellationToken>())
            .Throws(new TestDbException());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }
}

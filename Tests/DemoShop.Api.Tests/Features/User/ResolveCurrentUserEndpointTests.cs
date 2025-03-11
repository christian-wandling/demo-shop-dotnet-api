#region

using Ardalis.Result;
using DemoShop.Api.Features.User.Endpoints;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Processes.ResolveUser;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Api.Tests.Features.User;

[Trait("Feature", "User")]
public class ResolveCurrentUserEndpointTests : Test
{
    private readonly IMediator _mediator;
    private readonly ResolveCurrentUserEndpoint _sut;

    public ResolveCurrentUserEndpointTests()
    {
        _mediator = Mock<IMediator>();
        var logger = Mock<ILogger>();
        _sut = new ResolveCurrentUserEndpoint(_mediator, logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUserResponse_WhenQuerySucceeds()
    {
        // Arrange
        var expectedResponse = Create<Result<UserResponse>>();
        _mediator.Send(Arg.Any<ResolveUserProcess>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        await _mediator.Received(1)
            .Send(Arg.Is<ResolveUserProcess>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateException_WhenMediatorThrows()
    {
        // Arrange
        var expectedException = new Exception("Mediator error");
        _mediator.Send(Arg.Any<ResolveUserProcess>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(expectedException);

        // Act
        var act = () => _sut.HandleAsync();

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnExpectedFailureStatus_WhenMediatorReturnsFailure()
    {
        // Arrange
        _mediator.Send(Arg.Any<ResolveUserProcess>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Error message"));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.IsSuccess.Should().BeFalse();
    }
}

#region

using Ardalis.Result;
using DemoShop.Api.Features.ShoppingSession.Endpoints;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Processes.ResolveShoppingSession;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Api.Tests.Features.ShoppingSession;

[Trait("Feature", "ShoppingSession")]
public class ResolveCurrentShoppingSessionEndpointTests : Test
{
    private readonly IMediator _mediator;
    private readonly ResolveCurrentShoppingSessionEndpoint _sut;

    public ResolveCurrentShoppingSessionEndpointTests()
    {
        _mediator = Mock<IMediator>();
        var logger = Mock<ILogger>();
        _sut = new ResolveCurrentShoppingSessionEndpoint(_mediator, logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCartItemResponse_WhenQuerySucceeds()
    {
        // Arrange
        var expectedResponse = Create<Result<ShoppingSessionResponse>>();
        _mediator.Send(Arg.Any<ResolveShoppingSessionProcess>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        await _mediator.Received(1)
            .Send(Arg.Is<ResolveShoppingSessionProcess>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateException_WhenMediatorThrows()
    {
        // Arrange
        var expectedException = new Exception("Mediator error");
        _mediator.Send(Arg.Any<ResolveShoppingSessionProcess>(), Arg.Any<CancellationToken>())
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
        _mediator.Send(Arg.Any<ResolveShoppingSessionProcess>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Error message"));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.IsSuccess.Should().BeFalse();
    }
}

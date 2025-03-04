#region

using Ardalis.Result;
using DemoShop.Api.Features.ShoppingSession.Endpoints;
using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Features.ShoppingSession.Commands.UpdateCartItemQuantity;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Api.Tests.Features.ShoppingSession;

[Trait("Category", "Unit")]
[Trait("Layer", "Api")]
[Trait("Feature", "ShoppingSession")]
public class UpdateCartItemQuantityEndpointTests : Test
{
    private readonly IMediator _mediator;
    private readonly UpdateCartItemQuantityEndpoint _sut;

    public UpdateCartItemQuantityEndpointTests()
    {
        _mediator = Mock<IMediator>();
        var logger = Mock<ILogger>();
        _sut = new UpdateCartItemQuantityEndpoint(_mediator, logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCartItemResponse_WhenQuerySucceeds()
    {
        // Arrange
        var expectedResponse = Create<Result<UpdateCartItemQuantityResponse>>();
        var request = Create<UpdateCartItemQuantityRequestWrapper>();
        _mediator.Send(Arg.Any<UpdateCartItemQuantityCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        await _mediator.Received(1)
            .Send(Arg.Is<UpdateCartItemQuantityCommand>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateException_WhenMediatorThrows()
    {
        // Arrange
        var expectedException = new Exception("Mediator error");
        var request = Create<UpdateCartItemQuantityRequestWrapper>();
        _mediator.Send(Arg.Any<UpdateCartItemQuantityCommand>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(expectedException);

        // Act
        var act = () => _sut.HandleAsync(request);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage(expectedException.Message);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnExpectedFailureStatus_WhenMediatorReturnsFailure()
    {
        // Arrange
        var request = Create<UpdateCartItemQuantityRequestWrapper>();
        _mediator.Send(Arg.Any<UpdateCartItemQuantityCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Error message"));

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.IsSuccess.Should().BeFalse();
    }
}

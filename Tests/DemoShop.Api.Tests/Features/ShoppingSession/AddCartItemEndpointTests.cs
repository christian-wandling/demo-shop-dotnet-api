#region

using Ardalis.Result;
using DemoShop.Api.Features.ShoppingSession.Endpoints;
using DemoShop.Api.Features.ShoppingSession.Models;
using DemoShop.Application.Features.ShoppingSession.Commands.AddCartItem;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using NSubstitute.ExceptionExtensions;

#endregion

namespace DemoShop.Api.Tests.Features.ShoppingSession;

[Trait("Category", "Unit")]
[Trait("Layer", "Api")]
[Trait("Feature", "ShoppingSession")]
public class AddCartItemEndpointTests : Test
{
    private readonly IMediator _mediator;
    private readonly AddCartItemEndpoint _sut;

    public AddCartItemEndpointTests()
    {
        _mediator = Mock<IMediator>();
        _sut = new AddCartItemEndpoint(_mediator);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCartItemResponse_WhenQuerySucceeds()
    {
        // Arrange
        var expectedResponse = Create<Result<CartItemResponse>>();
        var request = Create<AddCartItemRequest>();
        _mediator.Send(Arg.Any<AddCartItemCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        await _mediator.Received(1)
            .Send(Arg.Is<AddCartItemCommand>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateException_WhenMediatorThrows()
    {
        // Arrange
        var expectedException = new Exception("Mediator error");
        var request = Create<AddCartItemRequest>();
        _mediator.Send(Arg.Any<AddCartItemCommand>(), Arg.Any<CancellationToken>())
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
        var request = Create<AddCartItemRequest>();
        _mediator.Send(Arg.Any<AddCartItemCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Error message"));

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.IsSuccess.Should().BeFalse();
    }
}

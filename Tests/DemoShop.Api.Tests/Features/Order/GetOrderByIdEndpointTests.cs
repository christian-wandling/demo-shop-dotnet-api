#region

using Ardalis.Result;
using DemoShop.Api.Features.Order.Endpoints;
using DemoShop.Api.Features.Order.Models;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Order.Queries.GetOrderById;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using NSubstitute.ExceptionExtensions;

#endregion

namespace DemoShop.Api.Tests.Features.Order;

[Trait("Category", "Unit")]
[Trait("Layer", "Api")]
[Trait("Feature", "Order")]
public class GetOrderByIdEndpointTests : Test
{
    private readonly GetOrderByIdEndpoint _sut;
    private readonly IMediator _mediator;

    public GetOrderByIdEndpointTests()
    {
        _mediator = Mock<IMediator>();
        _sut = new GetOrderByIdEndpoint(_mediator);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUserResponse_WhenQuerySucceeds()
    {
        // Arrange
        var request = Create<GetOrderByIdRequest>();
        var expectedResponse = Create<Result<OrderResponse>>();
        _mediator.Send(Arg.Any<GetOrderByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        await _mediator.Received(1)
            .Send(Arg.Is<GetOrderByIdQuery>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateException_WhenMediatorThrows()
    {
        // Arrange
        var request = Create<GetOrderByIdRequest>();
        var expectedException = new Exception("Mediator error");
        _mediator.Send(Arg.Any<GetOrderByIdQuery>(), Arg.Any<CancellationToken>())
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
        var request = Create<GetOrderByIdRequest>();
        _mediator.Send(Arg.Any<GetOrderByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Error message"));

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.IsSuccess.Should().BeFalse();
    }
}

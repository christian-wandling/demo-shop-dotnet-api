#region

using Ardalis.Result;
using DemoShop.Api.Features.Order.Endpoints;
using DemoShop.Application.Features.Order.Queries.GetAllOrdersOfUser;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using NSubstitute.ExceptionExtensions;

#endregion

namespace DemoShop.Api.Tests.Features.Order;

[Trait("Category", "Unit")]
[Trait("Layer", "Api")]
[Trait("Feature", "Order")]
public class GetAllOrdersOfUserEndpointTests : Test
{
    private readonly GetAllOrdersOfUserEndpoint _sut;
    private readonly IMediator _mediator;

    public GetAllOrdersOfUserEndpointTests()
    {
        _mediator = Mock<IMediator>();
        _sut = new GetAllOrdersOfUserEndpoint(_mediator);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUserResponse_WhenQuerySucceeds()
    {
        // Arrange
        var expectedResponse = Create<Result<OrderListResponse>>();
        _mediator.Send(Arg.Any<GetAllOrdersOfUserQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        await _mediator.Received(1)
            .Send(Arg.Is<GetAllOrdersOfUserQuery>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateException_WhenMediatorThrows()
    {
        // Arrange
        var expectedException = new Exception("Mediator error");
        _mediator.Send(Arg.Any<GetAllOrdersOfUserQuery>(), Arg.Any<CancellationToken>())
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
        _mediator.Send(Arg.Any<GetAllOrdersOfUserQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Error message"));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.IsSuccess.Should().BeFalse();
    }
}

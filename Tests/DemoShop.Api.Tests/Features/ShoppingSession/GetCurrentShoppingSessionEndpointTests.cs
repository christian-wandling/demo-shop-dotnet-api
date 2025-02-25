#region

using Ardalis.Result;
using DemoShop.Api.Features.ShoppingSession.Endpoints;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Queries.GetOrCreateShoppingSession;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using NSubstitute.ExceptionExtensions;

#endregion

namespace DemoShop.Api.Tests.Features.ShoppingSession;

[Trait("Category", "Unit")]
[Trait("Layer", "Api")]
[Trait("Feature", "ShoppingSession")]
public class GetCurrentShoppingSessionEndpointTests : Test
{
    private readonly IMediator _mediator;
    private readonly GetCurrentShoppingSessionEndpoint _sut;

    public GetCurrentShoppingSessionEndpointTests()
    {
        _mediator = Mock<IMediator>();
        _sut = new GetCurrentShoppingSessionEndpoint(_mediator);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCartItemResponse_WhenQuerySucceeds()
    {
        // Arrange
        var expectedResponse = Create<Result<ShoppingSessionResponse>>();
        _mediator.Send(Arg.Any<GetOrCreateShoppingSessionQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        await _mediator.Received(1)
            .Send(Arg.Is<GetOrCreateShoppingSessionQuery>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateException_WhenMediatorThrows()
    {
        // Arrange
        var expectedException = new Exception("Mediator error");
        _mediator.Send(Arg.Any<GetOrCreateShoppingSessionQuery>(), Arg.Any<CancellationToken>())
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
        _mediator.Send(Arg.Any<GetOrCreateShoppingSessionQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Error message"));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.IsSuccess.Should().BeFalse();
    }
}

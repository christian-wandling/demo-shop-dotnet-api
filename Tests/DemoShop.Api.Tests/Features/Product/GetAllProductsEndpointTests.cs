#region

using Ardalis.Result;
using DemoShop.Api.Features.Product.Endpoints;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.Product.Queries.GetAllProducts;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Api.Tests.Features.Product;

[Trait("Category", "Unit")]
[Trait("Layer", "Api")]
[Trait("Feature", "Product")]
public class GetAllProductsEndpointTests : Test
{
    private readonly IMediator _mediator;
    private readonly GetAllProductsEndpoint _sut;

    public GetAllProductsEndpointTests()
    {
        _mediator = Mock<IMediator>();
        var logger = Mock<ILogger>();
        _sut = new GetAllProductsEndpoint(_mediator, logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUserResponse_WhenQuerySucceeds()
    {
        // Arrange
        var expectedResponse = Create<Result<ProductListResponse>>();
        _mediator.Send(Arg.Any<GetAllProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        await _mediator.Received(1)
            .Send(Arg.Is<GetAllProductsQuery>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateException_WhenMediatorThrows()
    {
        // Arrange
        var expectedException = new Exception("Mediator error");
        _mediator.Send(Arg.Any<GetAllProductsQuery>(), Arg.Any<CancellationToken>())
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
        _mediator.Send(Arg.Any<GetAllProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Error message"));

        // Act
        var result = await _sut.HandleAsync();

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.IsSuccess.Should().BeFalse();
    }
}

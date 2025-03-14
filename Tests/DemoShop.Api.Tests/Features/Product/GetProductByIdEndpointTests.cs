#region

using Ardalis.Result;
using DemoShop.Api.Features.Product.Endpoints;
using DemoShop.Api.Features.Product.Models;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.Product.Queries.GetProductById;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Api.Tests.Features.Product;

[Trait("Feature", "Product")]
public class GetProductByIdEndpointTests : Test
{
    private readonly IMediator _mediator;
    private readonly GetProductByIdEndpoint _sut;

    public GetProductByIdEndpointTests()
    {
        _mediator = Mock<IMediator>();
        var logger = Mock<ILogger>();
        _sut = new GetProductByIdEndpoint(_mediator, logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUserResponse_WhenQuerySucceeds()
    {
        // Arrange
        var request = Create<GetProductByIdRequest>();
        var expectedResponse = Create<Result<ProductResponse>>();
        _mediator.Send(Arg.Any<GetProductByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        await _mediator.Received(1)
            .Send(Arg.Is<GetProductByIdQuery>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateException_WhenMediatorThrows()
    {
        // Arrange
        var request = Create<GetProductByIdRequest>();
        var expectedException = new Exception("Mediator error");
        _mediator.Send(Arg.Any<GetProductByIdQuery>(), Arg.Any<CancellationToken>())
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
        var request = Create<GetProductByIdRequest>();
        _mediator.Send(Arg.Any<GetProductByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Error message"));

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.IsSuccess.Should().BeFalse();
    }
}

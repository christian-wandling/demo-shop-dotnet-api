#region

using Ardalis.Result;
using DemoShop.Api.Features.User.Endpoints;
using DemoShop.Api.Features.User.Models;
using DemoShop.Application.Features.User.Commands.UpdateUserAddress;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Api.Tests.Features.User;

[Trait("Feature", "User")]
public class UpdateCurrentUserAddressEndpointTests : Test
{
    private readonly IMediator _mediator;
    private readonly UpdateCurrentUserAddressEndpoint _sut;

    public UpdateCurrentUserAddressEndpointTests()
    {
        _mediator = Mock<IMediator>();
        var logger = Mock<ILogger>();
        _sut = new UpdateCurrentUserAddressEndpoint(_mediator, logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAddressResponse_WhenQuerySucceeds()
    {
        // Arrange
        var expectedResponse = Create<Result<AddressResponse>>();
        var request = Create<UpdateUserAddressRequest>();
        _mediator.Send(Arg.Any<UpdateUserAddressCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        await _mediator.Received(1)
            .Send(Arg.Is<UpdateUserAddressCommand>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateException_WhenMediatorThrows()
    {
        // Arrange
        var expectedException = new Exception("Mediator error");
        var request = Create<UpdateUserAddressRequest>();
        _mediator.Send(Arg.Any<UpdateUserAddressCommand>(), Arg.Any<CancellationToken>())
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
        var request = Create<UpdateUserAddressRequest>();
        _mediator.Send(Arg.Any<UpdateUserAddressCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Error message"));

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.IsSuccess.Should().BeFalse();
    }
}

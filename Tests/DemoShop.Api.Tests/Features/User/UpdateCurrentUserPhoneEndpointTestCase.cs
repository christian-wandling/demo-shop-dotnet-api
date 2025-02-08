#region

using Ardalis.Result;
using DemoShop.Api.Features.User.Endpoints;
using DemoShop.Api.Features.User.Models;
using DemoShop.Application.Features.User.Commands.UpdateUserPhone;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using NSubstitute.ExceptionExtensions;

#endregion

namespace DemoShop.Api.Tests.Features.User;

[Trait("Category", "Unit")]
[Trait("Layer", "Api")]
[Trait("Feature", "User")]
public class UpdateCurrentUserPhoneEndpointTests : Test
{
    private readonly UpdateCurrentUserPhoneEndpoint _sut;
    private readonly IMediator _mediator;

    public UpdateCurrentUserPhoneEndpointTests()
    {
        _mediator = Mock<IMediator>();
        _sut = new UpdateCurrentUserPhoneEndpoint(_mediator);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUserPhoneResponse_WhenQuerySucceeds()
    {
        // Arrange
        var expectedResponse = Create<Result<UserPhoneResponse>>();
        var request = Create<UpdateUserPhoneRequest>();
        _mediator.Send(Arg.Any<UpdateUserPhoneCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        await _mediator.Received(1)
            .Send(Arg.Is<UpdateUserPhoneCommand>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPropagateException_WhenMediatorThrows()
    {
        // Arrange
        var expectedException = new Exception("Mediator error");
        var request = Create<UpdateUserPhoneRequest>();
        _mediator.Send(Arg.Any<UpdateUserPhoneCommand>(), Arg.Any<CancellationToken>())
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
        var request = Create<UpdateUserPhoneRequest>();
        _mediator.Send(Arg.Any<UpdateUserPhoneCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Error("Error message"));

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.IsSuccess.Should().BeFalse();
    }
}

#region

using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Common.Exceptions;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

#endregion

namespace DemoShop.Application.Tests.Features.User.Queries;

public class GetUserByKeycloakIdQueryHandlerTests : Test
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;
    private readonly GetUserByKeycloakIdQueryHandler _sut;

    public GetUserByKeycloakIdQueryHandlerTests()
    {
        _mapper = Mock<IMapper>();
        _repository = Mock<IUserRepository>();
        var logger = Mock<ILogger<GetUserByKeycloakIdQueryHandler>>();

        _sut = new GetUserByKeycloakIdQueryHandler(_mapper, _repository, logger);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = Create<GetUserByKeycloakIdQuery>();
        var user = Create<UserEntity>();
        var userResponse = Create<UserResponse>();

        _repository.GetUserByKeycloakIdAsync(query.KeycloakUserId, Arg.Any<CancellationToken>())
            .Returns(user);
        _mapper.Map<UserResponse>(user).Returns(userResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(userResponse);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnErrorResult()
    {
        // Arrange
        var query = Create<GetUserByKeycloakIdQuery>();

        _repository.GetUserByKeycloakIdAsync(query.KeycloakUserId, Arg.Any<CancellationToken>())
            .Returns((UserEntity?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ShouldReturnErrorResult()
    {
        // Arrange
        var query = Create<GetUserByKeycloakIdQuery>();
        const string exceptionMessage = "Invalid operation";

        _repository.GetUserByKeycloakIdAsync(query.KeycloakUserId, Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException(exceptionMessage));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenDbExceptionOccurs_ShouldReturnErrorResult()
    {
        // Arrange
        var query = Create<GetUserByKeycloakIdQuery>();

        _repository.GetUserByKeycloakIdAsync(query.KeycloakUserId, Arg.Any<CancellationToken>())
            .Throws(new TestDbException());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenRequestIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        GetUserByKeycloakIdQuery? query = null;

        // Act
        var act = () => _sut.Handle(query!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}

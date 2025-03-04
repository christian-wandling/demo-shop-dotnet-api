#region

using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.User.DTOs;
using DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;
using DemoShop.Domain.User.Entities;
using DemoShop.Domain.User.Interfaces;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Common.Exceptions;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Application.Tests.Features.User.Queries;

public class GetUserByKeycloakIdQueryHandlerTests : Test
{
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;
    private readonly GetUserByKeycloakIdQueryHandler _sut;

    public GetUserByKeycloakIdQueryHandlerTests()
    {
        _mapper = Mock<IMapper>();
        _repository = Mock<IUserRepository>();
        var logger = Mock<ILogger>();
        _cacheService = Mock<ICacheService>();
        _sut = new GetUserByKeycloakIdQueryHandler(_mapper, _repository, logger, _cacheService);
    }

    [Fact]
    public async Task Handle_WhenUserExistsInCache_ShouldReturnSuccessResultFromCache()
    {
        // Arrange
        var query = Create<GetUserByKeycloakIdQuery>();
        var userResponse = Create<UserResponse>();
        var cacheKey = Create<string>();

        _cacheService.GenerateCacheKey("user", query).Returns(cacheKey);
        _cacheService.GetFromCache<UserResponse>(cacheKey).Returns(userResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(userResponse);
        await _repository.DidNotReceive().GetUserByKeycloakIdAsync(query.KeycloakUserId, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenUserExistsNotInCache_ShouldReturnSuccessResultFromDatabase()
    {
        // Arrange
        var query = Create<GetUserByKeycloakIdQuery>();
        var user = Create<UserEntity>();
        var userResponse = Create<UserResponse>();
        var cacheKey = Create<string>();

        _cacheService.GenerateCacheKey("user", query).Returns(cacheKey);
        _cacheService.GetFromCache<UserResponse>(cacheKey).Returns((UserResponse?)null);

        _repository.GetUserByKeycloakIdAsync(query.KeycloakUserId, Arg.Any<CancellationToken>())
            .Returns(user);
        _mapper.Map<UserResponse>(user).Returns(userResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(userResponse);
        await _repository.Received(1).GetUserByKeycloakIdAsync(query.KeycloakUserId, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnErrorResult()
    {
        // Arrange
        var query = Create<GetUserByKeycloakIdQuery>();
        var cacheKey = Create<string>();

        _cacheService.GenerateCacheKey("user", query).Returns(cacheKey);
        _cacheService.GetFromCache<UserResponse>(cacheKey).Returns((UserResponse?)null);
        _repository.GetUserByKeycloakIdAsync(query.KeycloakUserId, Arg.Any<CancellationToken>())
            .Returns((UserEntity?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ShouldReturnErrorResult()
    {
        // Arrange
        var query = Create<GetUserByKeycloakIdQuery>();
        const string exceptionMessage = "Invalid operation";
        var cacheKey = Create<string>();

        _cacheService.GenerateCacheKey("user", query).Returns(cacheKey);
        _cacheService.GetFromCache<UserResponse>(cacheKey).Returns((UserResponse?)null);
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
        var cacheKey = Create<string>();

        _cacheService.GenerateCacheKey("user", query).Returns(cacheKey);
        _cacheService.GetFromCache<UserResponse>(cacheKey).Returns((UserResponse?)null);
        _repository.GetUserByKeycloakIdAsync(query.KeycloakUserId, Arg.Any<CancellationToken>())
            .Throws(new TestDbException());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);

        await _repository.Received(1).GetUserByKeycloakIdAsync(query.KeycloakUserId, CancellationToken.None);
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

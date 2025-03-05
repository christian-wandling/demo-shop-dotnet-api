#region

using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.ShoppingSession.DTOs;
using DemoShop.Application.Features.ShoppingSession.Queries.GetShoppingSessionByUserId;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.Domain.ShoppingSession.Interfaces;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Common.Exceptions;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Application.Tests.Features.ShoppingSession.Queries;

public class GetShoppingSessionByUserIdQueryHandlerTests : Test
{
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IShoppingSessionRepository _repository;
    private readonly GetShoppingSessionByUserIdQueryHandler _sut;

    public GetShoppingSessionByUserIdQueryHandlerTests()
    {
        _mapper = Substitute.For<IMapper>();
        _repository = Substitute.For<IShoppingSessionRepository>();
        _logger = Substitute.For<ILogger>();
        _cacheService = Substitute.For<ICacheService>();
        _sut = new GetShoppingSessionByUserIdQueryHandler(_mapper, _repository, _logger, _cacheService);
    }

    [Fact]
    public async Task Handle_WhenRequestIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        GetShoppingSessionByUserIdQuery request = null!;

        // Act
        var act = () => _sut.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName(nameof(request));
    }

    [Fact]
    public async Task Handle_WhenSessionExistsInCache_ReturnsSuccessResultFromCache()
    {
        // Arrange
        var request = Create<GetShoppingSessionByUserIdQuery>();
        var expectedResponse = Create<ShoppingSessionResponse>();
        var cacheKey = Create<string>();

        _cacheService.GenerateCacheKey("shoppingSession", request).Returns(cacheKey);
        _cacheService.GetFromCache<ShoppingSessionResponse>(cacheKey).Returns(expectedResponse);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResponse);
        await _repository.DidNotReceive().GetSessionByUserIdAsync(request.UserId, Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Handle_WhenSessionExistsButNotInCache_ReturnsSuccessResultFromDatabase()
    {
        // Arrange
        var request = Create<GetShoppingSessionByUserIdQuery>();
        var shoppingSession = Create<ShoppingSessionEntity>();
        var expectedResponse = Create<ShoppingSessionResponse>();
        var cacheKey = Create<string>();

        _cacheService.GenerateCacheKey("shoppingSession", request).Returns(cacheKey);
        _cacheService.GetFromCache<ShoppingSessionResponse>(cacheKey).Returns((ShoppingSessionResponse)null);

        _repository.GetSessionByUserIdAsync(request.UserId, Arg.Any<CancellationToken>())
            .Returns(shoppingSession);
        _mapper.Map<ShoppingSessionResponse>(shoppingSession)
            .Returns(expectedResponse);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResponse);
        await _repository.Received(1).GetSessionByUserIdAsync(request.UserId, Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Handle_WhenSessionNotFound_ReturnsErrorResult()
    {
        // Arrange
        var request = Create<GetShoppingSessionByUserIdQuery>();
        _repository.GetSessionByUserIdAsync(request.UserId, Arg.Any<CancellationToken>())
            .Returns((ShoppingSessionEntity)null!);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ReturnsErrorResult()
    {
        // Arrange
        var request = Create<GetShoppingSessionByUserIdQuery>();
        var exception = new InvalidOperationException("Invalid operation");
        var cacheKey = Create<string>();

        _cacheService.GenerateCacheKey("shoppingSession", request).Returns(cacheKey);
        _cacheService.GetFromCache<ShoppingSessionResponse>(cacheKey).Returns((ShoppingSessionResponse)null);
        _repository.GetSessionByUserIdAsync(request.UserId, Arg.Any<CancellationToken>())
            .Throws(exception);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenDbExceptionOccurs_ReturnsErrorResult()
    {
        // Arrange
        var request = Create<GetShoppingSessionByUserIdQuery>();
        var exception = new TestDbException("Database error");
        var cacheKey = Create<string>();

        _cacheService.GenerateCacheKey("shoppingSession", request).Returns(cacheKey);
        _cacheService.GetFromCache<ShoppingSessionResponse>(cacheKey).Returns((ShoppingSessionResponse)null);
        _repository.GetSessionByUserIdAsync(request.UserId, Arg.Any<CancellationToken>())
            .Throws(exception);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        await _repository.Received(1).GetSessionByUserIdAsync(request.UserId, Arg.Any<CancellationToken>());
    }
}

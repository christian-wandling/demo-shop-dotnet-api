#region

using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Order.Queries.GetOrderById;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Interfaces;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Common.Exceptions;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Application.Tests.Features.Order.Queries;

[Trait("Feature", "Order")]
public class GetOrderByIdQueryHandlerTests : Test
{
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly IOrderRepository _repository;
    private readonly GetOrderByIdQueryHandler _sut;
    private readonly ICurrentUserAccessor _userAccessor;

    public GetOrderByIdQueryHandlerTests()
    {
        _userAccessor = Mock<ICurrentUserAccessor>();
        _mapper = Mock<IMapper>();
        var logger = Mock<ILogger>();
        _repository = Mock<IOrderRepository>();
        _cacheService = Mock<ICacheService>();

        _sut = new GetOrderByIdQueryHandler(
            _userAccessor,
            _mapper,
            logger,
            _repository,
            _cacheService
        );
    }

    [Fact]
    public async Task Handle_WhenUserAuthorizationFails_ReturnsForbiddenResult()
    {
        // Arrange
        var query = new GetOrderByIdQuery(1);
        _userAccessor.GetId(CancellationToken.None).Returns(Result.Forbidden("Auth failed"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Forbidden);
    }

    [Fact]
    public async Task Handle_WhenOrderNotInCacheButExists_ReturnsSuccessWithMappedResponseFromDatabase()
    {
        // Arrange
        var userId = Create<int>();
        var query = new GetOrderByIdQuery(1);
        var order = Create<OrderEntity>();
        var mappedResponse = Create<OrderResponse>();
        var cacheKey = Create<string>();

        _userAccessor.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _cacheService.GenerateCacheKey("order", query).Returns(cacheKey);
        _cacheService.GetFromCache<OrderResponse>(cacheKey).Returns((OrderResponse?)null);
        _repository.GetOrderByIdAsync(query.Id, userId, CancellationToken.None).Returns(order);
        _mapper.Map<OrderResponse>(order).Returns(mappedResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(mappedResponse);
        await _repository.Received(1).GetOrderByIdAsync(query.Id, userId, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenOrderExistsInCache_ReturnsSuccessWithMappedResponseFromCache()
    {
        // Arrange
        var userId = Create<int>();
        var query = new GetOrderByIdQuery(1);
        var mappedResponse = Create<OrderResponse>();
        var cacheKey = Create<string>();

        _userAccessor.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _cacheService.GenerateCacheKey("order", query).Returns(cacheKey);
        _cacheService.GetFromCache<OrderResponse>(cacheKey).Returns(mappedResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(mappedResponse);
        await _repository.DidNotReceive().GetOrderByIdAsync(query.Id, userId, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var userId = Create<int>();
        var query = new GetOrderByIdQuery(1);
        var cacheKey = Create<string>();

        _userAccessor.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _cacheService.GenerateCacheKey("order", query).Returns(cacheKey);
        _cacheService.GetFromCache<OrderResponse>(cacheKey).Returns((OrderResponse?)null);
        _repository.GetOrderByIdAsync(query.Id, userId, CancellationToken.None).Returns((OrderEntity?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        await _repository.Received(1).GetOrderByIdAsync(query.Id, userId, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenDbExceptionOccurs_ReturnsErrorResult()
    {
        // Arrange
        var userId = Create<int>();
        var query = new GetOrderByIdQuery(1);
        var cacheKey = Create<string>();
        var exception = new TestDbException("Database error");

        _userAccessor.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _cacheService.GenerateCacheKey("order", query).Returns(cacheKey);
        _cacheService.GetFromCache<OrderResponse>(cacheKey).Returns((OrderResponse?)null);
        _repository.GetOrderByIdAsync(query.Id, userId, CancellationToken.None)
            .ThrowsAsync(exception);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        await _repository.Received(1).GetOrderByIdAsync(query.Id, userId, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ReturnsErrorResult()
    {
        // Arrange
        var userId = Create<int>();
        var query = new GetOrderByIdQuery(1);
        var cacheKey = Create<string>();
        var exception = new InvalidOperationException("Invalid operation");

        _userAccessor.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _cacheService.GenerateCacheKey("order", query).Returns(cacheKey);
        _cacheService.GetFromCache<OrderResponse>(cacheKey).Returns((OrderResponse?)null);
        _repository.GetOrderByIdAsync(query.Id, userId, CancellationToken.None)
            .ThrowsAsync(exception);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_WhenIdIsInvalid_ThrowsArgumentException(int invalidId)
    {
        // Arrange
        var query = new GetOrderByIdQuery(invalidId);

        // Act
        var act = () => _sut.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage(
                $"Required input {nameof(query.Id)} cannot be zero or negative. (Parameter '{nameof(query.Id)}')");
    }

    [Fact]
    public async Task Handle_WhenQueryIsNull_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.Handle(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");
    }
}

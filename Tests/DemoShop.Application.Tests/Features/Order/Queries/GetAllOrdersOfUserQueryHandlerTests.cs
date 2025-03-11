#region

using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Order.Queries.GetAllOrdersOfUser;
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
public class GetAllOrdersOfUserQueryHandlerTests : Test
{
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly IOrderRepository _repository;
    private readonly GetAllOrdersOfUserQueryHandler _sut;
    private readonly ICurrentUserAccessor _userAccessor;

    public GetAllOrdersOfUserQueryHandlerTests()
    {
        _userAccessor = Mock<ICurrentUserAccessor>();
        _mapper = Mock<IMapper>();
        _repository = Mock<IOrderRepository>();
        var logger = Mock<ILogger>();
        _cacheService = Mock<ICacheService>();

        _sut = new GetAllOrdersOfUserQueryHandler(
            _userAccessor,
            _mapper,
            _repository,
            logger,
            _cacheService
        );
    }

    [Fact]
    public async Task Handle_WhenUserIdIsInvalid_ReturnsUnauthorized()
    {
        // Arrange
        var query = Create<GetAllOrdersOfUserQuery>();
        _userAccessor.GetId(CancellationToken.None).Returns(Result.Unauthorized("Unauthorized"));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_WhenSuccessfulAndInCache_ReturnsOrderListResponseFromCache()
    {
        // Arrange
        var query = Create<GetAllOrdersOfUserQuery>();
        var userId = Create<int>();
        var mappedResponse = Create<OrderListResponse>();
        var cacheKey = Create<string>();

        _userAccessor.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _cacheService.GenerateCacheKey("orders-of-user", userId).Returns(cacheKey);
        _cacheService.GetFromCache<OrderListResponse>(cacheKey).Returns(mappedResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(mappedResponse);
        await _repository.DidNotReceive().GetOrdersByUserIdAsync(userId, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenSuccessfulAndNotInCache_ReturnsOrderListResponseFromDatabase()
    {
        // Arrange
        var query = Create<GetAllOrdersOfUserQuery>();
        var userId = Create<int>();
        var orders = Create<List<OrderEntity>>();
        var mappedResponse = Create<OrderListResponse>();
        var cacheKey = Create<string>();

        _userAccessor.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _cacheService.GenerateCacheKey("orders-of-user", userId).Returns(cacheKey);
        _cacheService.GetFromCache<OrderListResponse>(cacheKey).Returns((OrderListResponse?)null);
        _repository.GetOrdersByUserIdAsync(userId, CancellationToken.None).Returns(orders);
        _mapper.Map<OrderListResponse>(orders).Returns(mappedResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(mappedResponse);
        await _repository.Received(1).GetOrdersByUserIdAsync(userId, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ReturnsError()
    {
        // Arrange
        var query = Create<GetAllOrdersOfUserQuery>();
        var userId = Create<int>();
        var cacheKey = Create<string>();
        const string errorMessage = "Invalid operation occurred";

        _userAccessor.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _cacheService.GenerateCacheKey("orders-of-user", userId).Returns(cacheKey);
        _cacheService.GetFromCache<OrderListResponse>(cacheKey).Returns((OrderListResponse?)null);
        _repository.GetOrdersByUserIdAsync(userId, CancellationToken.None)
            .Throws(new InvalidOperationException(errorMessage));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenDbExceptionOccurs_ReturnsError()
    {
        // Arrange
        var query = Create<GetAllOrdersOfUserQuery>();
        var userId = Create<int>();
        var cacheKey = Create<string>();
        const string errorMessage = "Database error occurred";

        _userAccessor.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _cacheService.GenerateCacheKey("orders-of-user", userId).Returns(cacheKey);
        _cacheService.GetFromCache<OrderListResponse>(cacheKey).Returns((OrderListResponse?)null);
        _repository.GetOrdersByUserIdAsync(userId, CancellationToken.None)
            .Throws(new TestDbException(errorMessage));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        await _repository.Received(1).GetOrdersByUserIdAsync(userId, CancellationToken.None);
    }
}

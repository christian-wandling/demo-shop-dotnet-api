#region

using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.Order.Queries.GetAllOrdersOfUser;
using DemoShop.Application.Features.User.Interfaces;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.Order.Interfaces;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Common.Exceptions;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

#endregion

namespace DemoShop.Application.Tests.Features.Order.Queries;

public class GetAllOrdersOfUserQueryHandlerTests : Test
{
    private readonly ILogger<GetAllOrdersOfUserQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IOrderRepository _repository;
    private readonly GetAllOrdersOfUserQueryHandler _sut;
    private readonly ICurrentUserAccessor _userAccessor;

    public GetAllOrdersOfUserQueryHandlerTests()
    {
        _userAccessor = Mock<ICurrentUserAccessor>();
        _mapper = Mock<IMapper>();
        _repository = Mock<IOrderRepository>();
        _logger = Mock<ILogger<GetAllOrdersOfUserQueryHandler>>();

        _sut = new GetAllOrdersOfUserQueryHandler(
            _userAccessor,
            _mapper,
            _repository,
            _logger
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
    public async Task Handle_WhenSuccessful_ReturnsOrderListResponse()
    {
        // Arrange
        var query = Create<GetAllOrdersOfUserQuery>();
        var userId = Create<int>();
        var orders = Create<List<OrderEntity>>();
        var mappedResponse = Create<OrderListResponse>();

        _userAccessor.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _repository.GetOrdersByUserIdAsync(userId, CancellationToken.None).Returns(orders);
        _mapper.Map<OrderListResponse>(orders).Returns(mappedResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(mappedResponse);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ReturnsError()
    {
        // Arrange
        var query = Create<GetAllOrdersOfUserQuery>();
        var userId = Create<int>();
        const string errorMessage = "Invalid operation occurred";

        _userAccessor.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _repository.GetOrdersByUserIdAsync(userId, CancellationToken.None)
            .Throws(new InvalidOperationException(errorMessage));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        _logger.Received(1).LogDomainException(errorMessage);
    }

    [Fact]
    public async Task Handle_WhenDbExceptionOccurs_ReturnsError()
    {
        // Arrange
        var query = Create<GetAllOrdersOfUserQuery>();
        var userId = Create<int>();
        const string errorMessage = "Database error occurred";

        _userAccessor.GetId(CancellationToken.None).Returns(Result.Success(userId));
        _repository.GetOrdersByUserIdAsync(userId, CancellationToken.None)
            .Throws(new TestDbException(errorMessage));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        _logger.Received(1).LogOperationFailed(
            "Get all orders of user",
            "UserId",
            userId.ToString(),
            null);
    }
}

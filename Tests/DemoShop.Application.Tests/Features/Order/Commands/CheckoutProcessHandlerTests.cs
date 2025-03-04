#region

using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.Order.Commands.CreateOrder;
using DemoShop.Application.Features.Order.DTOs;
using DemoShop.Application.Features.ShoppingSession.Commands.DeleteShoppingSession;
using DemoShop.Application.Features.ShoppingSession.Interfaces;
using DemoShop.Application.Features.ShoppingSession.Processes.Checkout;
using DemoShop.Domain.Common.Interfaces;
using DemoShop.Domain.Order.Entities;
using DemoShop.Domain.ShoppingSession.Entities;
using DemoShop.TestUtils.Common.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Application.Tests.Features.Order.Commands;

public class CheckoutProcessHandlerTests : Test
{
    private readonly ICurrentShoppingSessionAccessor _currentSession;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CheckoutProcessHandler _sut;
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutProcessHandlerTests()
    {
        _currentSession = Mock<ICurrentShoppingSessionAccessor>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Mock<IMediator>();
        _logger = Mock<ILogger>();
        _eventDispatcher = Mock<IDomainEventDispatcher>();
        _unitOfWork = Mock<IUnitOfWork>();

        _sut = new CheckoutProcessHandler(
            _currentSession,
            _mapper,
            _mediator,
            _logger,
            _eventDispatcher,
            _unitOfWork
        );
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ReturnsOrderResponse()
    {
        // Arrange
        var command = Create<CheckoutProcess>();
        var session = Create<ShoppingSessionEntity>();
        var order = Create<OrderEntity>();
        var orderResponse = Create<OrderResponse>();

        _currentSession.GetCurrent(CancellationToken.None).Returns(Result.Success(session));

        _mediator.Send(Arg.Any<CreateOrderCommand>(), CancellationToken.None)
            .Returns(Result.Success(order));

        _mediator.Send(Arg.Any<DeleteShoppingSessionCommand>(), CancellationToken.None)
            .Returns(Result.Success());

        _mapper.Map<OrderResponse>(order).Returns(orderResponse);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(orderResponse);

        await _unitOfWork.Received(1).BeginTransactionAsync(CancellationToken.None);
        await _unitOfWork.Received(1).CommitTransactionAsync(CancellationToken.None);
        await _eventDispatcher.Received(1).DispatchEventsAsync(order, CancellationToken.None);
    }


    [Fact]
    public async Task Handle_WhenNoActiveSession_ReturnsNotFound()
    {
        // Arrange
        var command = Create<CheckoutProcess>();
        _currentSession.GetCurrent(CancellationToken.None).Returns(Result.NotFound("No session found"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_WhenConvertSessionToOrderFails_RollsBackTransaction()
    {
        // Arrange
        var command = Create<CheckoutProcess>();
        var session = Create<ShoppingSessionEntity>();
        const string errorMessage = "Conversion failed";

        _currentSession.GetCurrent(CancellationToken.None).Returns(Result.Success(session));

        _mediator.Send(Arg.Any<CreateOrderCommand>(), CancellationToken.None)
            .Returns(Result.Error(errorMessage));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);

        await _unitOfWork.Received(1).RollbackTransactionAsync(CancellationToken.None);
    }


    [Fact]
    public async Task Handle_WhenDeleteSessionFails_RollsBackTransaction()
    {
        // Arrange
        var command = Create<CheckoutProcess>();
        var session = Create<ShoppingSessionEntity>();
        var order = Create<OrderEntity>();
        const string errorMessage = "Delete failed";

        _currentSession.GetCurrent(CancellationToken.None).Returns(Result.Success(session));
        _mediator.Send(Arg.Any<CreateOrderCommand>(), CancellationToken.None)
            .Returns(Result.Success(order));
        _mediator.Send(Arg.Any<DeleteShoppingSessionCommand>(), CancellationToken.None)
            .Returns(Result.Error(errorMessage));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);

        await _unitOfWork.Received().RollbackTransactionAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenDbUpdateExceptionOccurs_LogsAndReturnsError()
    {
        // Arrange
        var command = Create<CheckoutProcess>();
        var session = Create<ShoppingSessionEntity>();
        var exception = new DbUpdateException("Database error");

        _currentSession.GetCurrent(CancellationToken.None).Returns(Result.Success(session));

        _mediator.Send(Arg.Any<CreateOrderCommand>(), CancellationToken.None)
            .Throws(exception);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.CriticalError);

        await _unitOfWork.Received(1).RollbackTransactionAsync(CancellationToken.None);
        // Verify the error is logged with the new logging pattern
        _logger.Received(1).Error(
            Arg.Is<Exception>(e => e == exception),
            Arg.Is<string>(s => s.Contains("Unhandled exception")),
            Arg.Any<string>(),
            Arg.Any<object>());
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_LogsAndReturnsError()
    {
        // Arrange
        var command = Create<CheckoutProcess>();
        var session = Create<ShoppingSessionEntity>();
        const string errorMessage = "Invalid operation";
        var exception = new InvalidOperationException(errorMessage);

        _currentSession.GetCurrent(CancellationToken.None).Returns(Result.Success(session));

        _mediator.Send(Arg.Any<CreateOrderCommand>(), CancellationToken.None)
            .Throws(exception);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.CriticalError);

        await _unitOfWork.Received(1).RollbackTransactionAsync(CancellationToken.None);
        _logger.Received(1).Error(
            Arg.Is<Exception>(e => e == exception),
            Arg.Is<string>(s => s.Contains("Unhandled exception")),
            Arg.Any<string>(),
            Arg.Any<object>());
    }
}

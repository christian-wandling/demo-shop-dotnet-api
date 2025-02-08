#region

using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.Product.Queries.GetProductById;
using DemoShop.Application.Features.User.Queries.GetUserByKeycloakId;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Product.Interfaces;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Common.Exceptions;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

#endregion

namespace DemoShop.Application.Tests.Features.Product.Queries;

public class GetProductByIdQueryHandlerTests : Test
{
    private readonly ILogger<GetUserByKeycloakIdQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IProductRepository _repository;
    private readonly GetProductByIdQueryHandler _sut;

    public GetProductByIdQueryHandlerTests()
    {
        _mapper = Mock<IMapper>();
        _repository = Mock<IProductRepository>();
        _logger = Mock<ILogger<GetUserByKeycloakIdQueryHandler>>();
        _sut = new GetProductByIdQueryHandler(_mapper, _repository, _logger);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ReturnsSuccessResult()
    {
        // Arrange
        var product = Create<ProductEntity>();
        var mappedResponse = Create<ProductResponse>();
        var query = new GetProductByIdQuery(1);

        _repository.GetProductByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _mapper.Map<ProductResponse>(product)
            .Returns(mappedResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(mappedResponse);

        await _repository.Received(1).GetProductByIdAsync(query.Id, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<ProductResponse>(product);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var query = new GetProductByIdQuery(1);

        _repository.GetProductByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns((ProductEntity?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);

        _logger.Received(1).LogOperationFailed(
            "Get Product By Id",
            "Id",
            $"{query.Id}",
            null);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ReturnsErrorResult()
    {
        // Arrange
        const string errorMessage = "Invalid operation error";
        var query = new GetProductByIdQuery(1);

        _repository.GetProductByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException(errorMessage));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);

        _logger.Received(1).LogDomainException(errorMessage);
    }

    [Fact]
    public async Task Handle_WhenDbExceptionOccurs_ReturnsErrorResult()
    {
        // Arrange
        const string errorMessage = "Database error";
        var query = new GetProductByIdQuery(1);

        _repository.GetProductByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Throws(new TestDbException(errorMessage));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);

        _logger.Received(1).LogOperationFailed(
            "Get product by Id",
            "Id",
            $"{query.Id}",
            null);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_WhenIdIsInvalid_ThrowsArgumentException(int invalidId)
    {
        // Arrange
        var query = new GetProductByIdQuery(invalidId);

        // Act
        var act = () => _sut.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("Id");
    }

    [Fact]
    public async Task Handle_WhenRequestIsNull_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.Handle(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");
    }
}

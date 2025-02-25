#region

using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.Product.Queries.GetAllProducts;
using DemoShop.Domain.Common.Logging;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Product.Interfaces;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Common.Exceptions;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

#endregion

namespace DemoShop.Application.Tests.Features.Product.Queries;

public class GetAllProductsQueryHandlerTests : Test
{
    private readonly ILogger<GetAllProductsQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IProductRepository _repository;
    private readonly GetAllProductsQueryHandler _sut;

    public GetAllProductsQueryHandlerTests()
    {
        _mapper = Mock<IMapper>();
        _repository = Mock<IProductRepository>();
        _logger = Mock<ILogger<GetAllProductsQueryHandler>>();
        _sut = new GetAllProductsQueryHandler(_mapper, _repository, _logger);
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ReturnsSuccessResult()
    {
        // Arrange
        var products = Create<List<ProductEntity>>();
        var mappedResponse = Create<ProductListResponse>();
        var query = Create<GetAllProductsQuery>();

        _repository.GetAllProductsAsync(Arg.Any<CancellationToken>())
            .Returns(products);

        _mapper.Map<ProductListResponse>(products)
            .Returns(mappedResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(mappedResponse);

        await _repository.Received(1).GetAllProductsAsync(Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<ProductListResponse>(products);
    }

    [Fact]
    public async Task Handle_WhenInvalidOperationExceptionOccurs_ReturnsErrorResult()
    {
        // Arrange
        const string errorMessage = "Invalid operation error";
        var query = Create<GetAllProductsQuery>();

        _repository.GetAllProductsAsync(Arg.Any<CancellationToken>())
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
        var query = Create<GetAllProductsQuery>();

        _repository.GetAllProductsAsync(Arg.Any<CancellationToken>())
            .Throws(new TestDbException(errorMessage));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);

        _logger.Received(1).LogOperationFailed(
            "Get all products",
            "",
            "",
            null
        );
    }
}

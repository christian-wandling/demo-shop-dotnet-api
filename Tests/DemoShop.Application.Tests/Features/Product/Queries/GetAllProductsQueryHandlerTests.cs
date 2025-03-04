#region

using Ardalis.Result;
using AutoMapper;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Application.Features.Product.DTOs;
using DemoShop.Application.Features.Product.Queries.GetAllProducts;
using DemoShop.Domain.Product.Entities;
using DemoShop.Domain.Product.Interfaces;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Common.Exceptions;
using NSubstitute.ExceptionExtensions;
using Serilog;

#endregion

namespace DemoShop.Application.Tests.Features.Product.Queries;

public class GetAllProductsQueryHandlerTests : Test
{
    private const string CacheKey = "all-products";
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IProductRepository _repository;
    private readonly GetAllProductsQueryHandler _sut;

    public GetAllProductsQueryHandlerTests()
    {
        _mapper = Mock<IMapper>();
        _repository = Mock<IProductRepository>();
        _logger = Mock<ILogger>();
        _cacheService = Mock<ICacheService>();
        _sut = new GetAllProductsQueryHandler(_mapper, _repository, _logger, _cacheService);
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ReturnsSuccessResult()
    {
        // Arrange
        var products = Create<List<ProductEntity>>();
        var mappedResponse = Create<ProductListResponse>();
        var query = Create<GetAllProductsQuery>();

        _cacheService.GenerateCacheKey("product", query)
            .Returns(CacheKey);
        _cacheService.GetFromCache<ProductListResponse>(CacheKey)
            .Returns((ProductListResponse?)null);

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

        _cacheService.GenerateCacheKey("product", query)
            .Returns(CacheKey);
        _cacheService.GetFromCache<ProductListResponse>(CacheKey)
            .Returns((ProductListResponse?)null);

        _repository.GetAllProductsAsync(Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException(errorMessage));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenDbExceptionOccurs_ReturnsErrorResult()
    {
        // Arrange
        const string errorMessage = "Database error";
        var query = Create<GetAllProductsQuery>();

        _cacheService.GenerateCacheKey("product", query)
            .Returns(CacheKey);
        _cacheService.GetFromCache<ProductListResponse>(CacheKey)
            .Returns((ProductListResponse?)null);

        _repository.GetAllProductsAsync(Arg.Any<CancellationToken>())
            .Throws(new TestDbException(errorMessage));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenExistsInCache_ReturnsSuccessResultFromCache()
    {
        // Arrange
        var query = new GetAllProductsQuery();
        var cachedResponse = Create<ProductListResponse>();

        _cacheService.GenerateCacheKey("product", query)
            .Returns(CacheKey);
        _cacheService.GetFromCache<ProductListResponse>(CacheKey)
            .Returns(cachedResponse);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(cachedResponse);

        // Verify cache is checked but repository is not called
        _cacheService.Received(1).GetFromCache<ProductListResponse>(CacheKey);
        await _repository.DidNotReceive().GetAllProductsAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNotInCacheButInRepository_SetsCache()
    {
        // Arrange
        var products = Create<List<ProductEntity>>();
        var mappedResponse = Create<ProductListResponse>();
        var query = new GetAllProductsQuery();

        _cacheService.GenerateCacheKey("product", query)
            .Returns(CacheKey);
        _cacheService.GetFromCache<ProductListResponse>(CacheKey)
            .Returns((ProductListResponse?)null);
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

        // Verify cache interaction
        _cacheService.Received(1).GetFromCache<ProductListResponse>(CacheKey);
        _cacheService.Received(1).SetCache(CacheKey, mappedResponse);
        await _repository.Received(1).GetAllProductsAsync(Arg.Any<CancellationToken>());
    }
}

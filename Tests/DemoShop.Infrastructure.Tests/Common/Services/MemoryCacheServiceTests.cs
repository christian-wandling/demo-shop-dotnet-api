using DemoShop.Domain.Common.Logging;
using DemoShop.Infrastructure.Common.Services;
using DemoShop.TestUtils.Common.Base;
using DemoShop.TestUtils.Common.Models;
using Xunit.Sdk;

namespace DemoShop.Infrastructure.Tests.Common.Services;

using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System;
using System.Text.Json;
using Xunit.Abstractions;

public class MemoryCacheServiceTests : Test
{
    private readonly MemoryCacheService _sut;
    private readonly IMemoryCache _cache;
    private readonly ILogger _logger;

    public MemoryCacheServiceTests(ITestOutputHelper? output = null) : base(output)
    {
        _cache = Mock<IMemoryCache>();
        _logger = Mock<ILogger>();
        _sut = new MemoryCacheService(_cache, _logger);
    }

    [Fact]
    public void GetFromCache_WhenKeyExists_ShouldReturnCachedItem()
    {
        // Arrange
        const string key = "test-key";
        var expectedItem = Create<TestClass>();

        object objExpectedItem = expectedItem;
        _cache.TryGetValue(key, out _)
            .Returns(x => {
                x[1] = objExpectedItem;
                return true;
            });

        // Act
        var result = _sut.GetFromCache<TestClass>(key);

        // Assert
        result.Should().Be(expectedItem);
        _logger.Received(1).Error(Arg.Any<Exception>(), Arg.Any<string>());
    }

    [Fact]
    public void GetFromCache_WhenKeyDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        const string key = "non-existent-key";
        _cache.TryGetValue(key, out _).Returns(false);

        // Act
        var result = _sut.GetFromCache<TestClass>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SetCache_ShouldAddItemToCache_WithProvidedExpirations()
    {
        // Arrange
        const string key = "test-key";
        var item = Create<TestClass>();
        var absoluteExpiration = TimeSpan.FromMinutes(15);
        var slidingExpiration = TimeSpan.FromMinutes(5);

        var cacheEntryMock = Substitute.For<ICacheEntry>();
        _cache.CreateEntry(key).Returns(cacheEntryMock);

        // Act
        _sut.SetCache(key, item, absoluteExpiration, slidingExpiration);

        // Assert
        _cache.Received(1).CreateEntry(key);
        cacheEntryMock.Received(1).SetAbsoluteExpiration(absoluteExpiration);
        cacheEntryMock.Received(1).SetSlidingExpiration(slidingExpiration);
        cacheEntryMock.Received(1).SetValue(item);
    }

    [Fact]
    public void SetCache_WithDefaultExpirations_ShouldUseDefaultValues()
    {
        // Arrange
        const string key = "test-key";
        var item = Create<TestClass>();

        var cacheEntryMock = Substitute.For<ICacheEntry>();
        _cache.CreateEntry(key).Returns(cacheEntryMock);

        // Act
        _sut.SetCache(key, item, null, null);

        // Assert
        _cache.Received(1).CreateEntry(key);
        cacheEntryMock.Received(1).SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
        cacheEntryMock.Received(1).SetSlidingExpiration(TimeSpan.FromMinutes(10));
        cacheEntryMock.Received(1).SetValue(item);
    }

    [Fact]
    public void GenerateCacheKey_ShouldCreateKeyWithCorrectFormat()
    {
        // Arrange
        const string prefix = "test";
        var request = new TestRequest { Name = "Test" };
        var expected = $"prefix--{request.GetType().Name}-{JsonSerializer.Serialize(request)}";

        // Act
        var result = _sut.GenerateCacheKey(prefix, request);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GenerateCacheKey_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        const string prefix = "test";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _sut.GenerateCacheKey(prefix, null!));
    }
}


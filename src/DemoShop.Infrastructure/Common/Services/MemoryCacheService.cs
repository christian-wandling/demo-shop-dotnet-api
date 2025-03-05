#region

using System.Text.Json;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

#endregion

namespace DemoShop.Infrastructure.Common.Services;

public class MemoryCacheService(IMemoryCache cache, ILogger logger) : ICacheService
{
    public string GenerateCacheKey(string prefix, object request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return $"{prefix}--{request.GetType().Name}-{JsonSerializer.Serialize(request)}";
    }

    public T? GetFromCache<T>(string key) where T : class
    {
        if (cache.TryGetValue(key, out T? cachedItem))
        {
            LogCacheHit(logger, key);
            return cachedItem;
        }

        LogCacheMiss(logger, key);
        return null;
    }

    public void SetCache<T>(string key, T item, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        where T : class
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions();

        cacheEntryOptions.SetAbsoluteExpiration(absoluteExpiration ?? TimeSpan.FromMinutes(30));
        cacheEntryOptions.SetSlidingExpiration(slidingExpiration ?? TimeSpan.FromMinutes(10));

        cache.Set(key, item, cacheEntryOptions);
        LogCacheWrite(logger, key);
    }

    public void InvalidateCache(string key)
    {
        cache.Remove(key);
        LogCacheInvalidate(logger, key);
    }

    private static void LogCacheWrite(ILogger logger, string key) =>
        logger.Information("[{EventId}] Cache write for {Key}", LoggerEventIds.CacheWrite, key);

    private static void LogCacheHit(ILogger logger, string key) =>
        logger.Information("[{EventId}] Cache hit for {Key}", LoggerEventIds.CacheHit, key);

    private static void LogCacheMiss(ILogger logger, string key) =>
        logger.Information("[{EventId}] Cache miss for {Key}", LoggerEventIds.CacheMiss, key);

    private static void LogCacheInvalidate(ILogger logger, string key) =>
        logger.Information("[{EventId}] Cache invalidate for {Key}", LoggerEventIds.CacheInvalidate, key);
}

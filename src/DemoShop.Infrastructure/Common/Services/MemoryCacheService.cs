using System.Text.Json;
using DemoShop.Application.Common.Interfaces;
using DemoShop.Domain.Common.Logging;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace DemoShop.Infrastructure.Common.Services;

public class MemoryCacheService(IMemoryCache cache, ILogger logger) : ICacheService
{
    public T? GetFromCache<T>(string key) where T : class
    {
        if (cache.TryGetValue(key, out T? cachedItem))
        {
            logger.LogCacheHit(key);
            return cachedItem;
        }

        logger.LogCacheMiss(key);
        return null;
    }

    public void SetCache<T>(string key, T item, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        where T : class
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions();

        cacheEntryOptions.SetAbsoluteExpiration(absoluteExpiration ?? TimeSpan.FromMinutes(30));
        cacheEntryOptions.SetSlidingExpiration(slidingExpiration ?? TimeSpan.FromMinutes(10));

        cache.Set(key, item, cacheEntryOptions);
        logger.LogCacheWrite(key);
    }

    public string GenerateCacheKey(string prefix, object request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return $"prefix--{request.GetType().Name}-{JsonSerializer.Serialize(request)}";
    }
}

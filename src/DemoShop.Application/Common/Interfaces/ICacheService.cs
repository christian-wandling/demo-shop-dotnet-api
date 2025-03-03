namespace DemoShop.Application.Common.Interfaces;

public interface ICacheService
{
    T? GetFromCache<T>(string key) where T : class;

    void SetCache<T>(string key, T item, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        where T : class;

    string GenerateCacheKey(string prefix, object request);

    void InvalidateCache(string key);
}

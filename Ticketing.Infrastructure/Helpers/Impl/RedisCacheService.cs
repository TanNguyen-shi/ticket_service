using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Ticketing.Infrastructure.Configurations.Options;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace Ticketing.Infrastructure.Helpers.Impl;

public class RedisCacheService(
    IDistributedCache distributedCache,
    IOptions<RedisCacheOptions> options)
    : ICacheService
{
    private readonly RedisCacheOptions _options = options.Value;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = null
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return default;

        var cached = await distributedCache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrWhiteSpace(cached))
            return default;

        return JsonSerializer.Deserialize<T>(cached, JsonOptions);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? ttl = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key) || value is null)
            return;

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl ?? TimeSpan.FromSeconds(Math.Max(_options.DefaultTtlSeconds, 1))
        };

        var serialized = JsonSerializer.Serialize(value, JsonOptions);
        await distributedCache.SetStringAsync(key, serialized, cacheOptions, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        await distributedCache.RemoveAsync(key, cancellationToken);
    }
}


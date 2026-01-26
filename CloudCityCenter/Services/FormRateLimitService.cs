using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace CloudCityCenter.Services;

public class FormRateLimitService
{
    private const int MaxSubmissions = 3;
    private static readonly TimeSpan BlockDuration = TimeSpan.FromHours(24);
    private static readonly TimeSpan CountDuration = TimeSpan.FromHours(24);
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new();

    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<FormRateLimitService> _logger;

    public FormRateLimitService(IMemoryCache memoryCache, IDistributedCache distributedCache, ILogger<FormRateLimitService> logger)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<bool> IsBlockedAsync(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return false;
        }

        var blockKey = GetBlockKey(ipAddress);
        if (_memoryCache.TryGetValue(blockKey, out _))
        {
            return true;
        }

        var cachedValue = await _distributedCache.GetStringAsync(blockKey);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            _memoryCache.Set(blockKey, true, BlockDuration);
            return true;
        }

        return false;
    }

    public async Task<bool> RegisterSubmissionAsync(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return false;
        }

        var gate = Locks.GetOrAdd(ipAddress, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync();
        try
        {
            if (await IsBlockedAsync(ipAddress))
            {
                return true;
            }

            var currentCount = await GetSubmissionCountAsync(ipAddress);
            currentCount++;

            if (currentCount >= MaxSubmissions)
            {
                await SetBlockedAsync(ipAddress);
                await ClearSubmissionCountAsync(ipAddress);
                _logger.LogWarning("IP {IpAddress} blocked after reaching {MaxSubmissions} form submissions.", ipAddress, MaxSubmissions);
                return true;
            }

            await SetSubmissionCountAsync(ipAddress, currentCount);
            return false;
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<int> GetSubmissionCountAsync(string ipAddress)
    {
        var countKey = GetCountKey(ipAddress);
        if (_memoryCache.TryGetValue(countKey, out int cachedCount))
        {
            return cachedCount;
        }

        var distributedValue = await _distributedCache.GetStringAsync(countKey);
        if (int.TryParse(distributedValue, out var parsedCount))
        {
            _memoryCache.Set(countKey, parsedCount, CountDuration);
            return parsedCount;
        }

        return 0;
    }

    private Task SetSubmissionCountAsync(string ipAddress, int count)
    {
        var countKey = GetCountKey(ipAddress);
        _memoryCache.Set(countKey, count, CountDuration);
        return _distributedCache.SetStringAsync(countKey, count.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CountDuration
        });
    }

    private Task ClearSubmissionCountAsync(string ipAddress)
    {
        var countKey = GetCountKey(ipAddress);
        _memoryCache.Remove(countKey);
        return _distributedCache.RemoveAsync(countKey);
    }

    private Task SetBlockedAsync(string ipAddress)
    {
        var blockKey = GetBlockKey(ipAddress);
        _memoryCache.Set(blockKey, true, BlockDuration);
        return _distributedCache.SetStringAsync(blockKey, "1", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = BlockDuration
        });
    }

    private static string GetBlockKey(string ipAddress)
    {
        return $"form-rate-limit:block:{ipAddress}";
    }

    private static string GetCountKey(string ipAddress)
    {
        return $"form-rate-limit:count:{ipAddress}";
    }
}

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace EC.GenericMemoryCache;

internal sealed class GenericMemoryCache<TKey, TValue>(IOptions<GenericMemoryCacheOptions> options) : IGenericMemoryCache<TKey, TValue>
    where TKey : notnull
{
    private const int ClearTagIdle = 0;
    private const int ClearTagClearing = 1;
    private int _clearCacheTag = ClearTagIdle;

    private DateTimeOffset _lastClearTime = DateTimeOffset.UtcNow;
    private readonly GenericMemoryCacheOptions _options = options.Value;

    private readonly ConcurrentDictionary<TKey, CacheEntry<TValue>> _cache = new();


    public void Set(TKey key, TValue value, TimeSpan? expire)
    {
        _cache[key] = new CacheEntry<TValue> { Value = value, Expire = expire, TimeProvider = _options.TimeProvider };
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_cache.TryGetValue(key, out var cacheEntry) == false)
        {
            value = default;
            return false;
        }

        if (cacheEntry.Expired)
        {
            value = default;
            _cache.Remove(key, out _);
            return false;
        }

        value = cacheEntry.Value;
        StartScanForExpiredItemsIfNeeded();
        return true;
    }

    public bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        value = default;
        var removeResult = _cache.TryRemove(key, out var cacheValue);

        if (removeResult)
        {
            value = cacheValue!.Value;
        }

        return removeResult;
    }


    private void StartScanForExpiredItemsIfNeeded()
    {
        var now = _options.TimeProvider.GetUtcNow();
        if (now - _lastClearTime > _options.ExpirationScanFrequency)
        {
            ScheduleTask(now);
        }

        void ScheduleTask(DateTimeOffset utcNow)
        {
            _lastClearTime = utcNow;
            Task.Factory.StartNew(ClearExpiredCache, TaskCreationOptions.LongRunning);
        }
    }

    private void ClearExpiredCache()
    {
        if (Interlocked.CompareExchange(ref _clearCacheTag, ClearTagClearing, ClearTagIdle) == ClearTagClearing)
        {
            return;
        }

        foreach (var (key, cacheEntry) in _cache)
        {
            if (cacheEntry.Expired)
            {
                _cache.TryRemove(key, out _);
            }
        }

        Interlocked.Exchange(ref _clearCacheTag, ClearTagIdle);
    }
}

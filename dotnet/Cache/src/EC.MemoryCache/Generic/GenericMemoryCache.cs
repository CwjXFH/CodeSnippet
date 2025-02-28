using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace MemoryCache.Generic;

internal sealed class GenericMemoryCache<TKey, TValue> where TKey : notnull
{
    private const int ClearTagIdle = 0;
    private const int ClearTagClearing = 1;
    private int _clearCacheTag = ClearTagIdle;
    private DateTime _lastClearTime = DateTime.UtcNow;
    private readonly TimeSpan _clearCacheInterval = TimeSpan.FromMinutes(30);
    private readonly ConcurrentDictionary<TKey, CacheEntry<TValue>> _cache = new();


    public void Set(TKey key, TValue value, TimeSpan? expire)
    {
        _cache[key] = new CacheEntry<TValue> { Value = value, Expire = expire };
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


    private void StartScanForExpiredItemsIfNeeded()
    {
        var now = DateTime.UtcNow;
        if (now - _lastClearTime > _clearCacheInterval)
        {
            ScheduleTask(now);
        }

        void ScheduleTask(DateTime utcNow)
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

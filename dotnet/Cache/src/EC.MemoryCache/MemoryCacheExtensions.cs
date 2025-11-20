using Microsoft.Extensions.Caching.Memory;
using MSCache = Microsoft.Extensions.Caching.Memory;

namespace MemoryCache;

public static class MemoryCacheExtensions
{
    /// <summary>
    /// Get the number of keys that are stored in the memory.
    /// </summary>
    /// <returns>-1 if failed, else the number of keys</returns>
    public static int KeyCount(this IMemoryCache memoryCache)
    {
        if (memoryCache is MSCache.MemoryCache cache)
        {
            return cache.Count;
        }

        return -1;
    }

    /// <summary>
    /// Return all memory cache keys
    /// </summary>
    public static IEnumerable<object> GetAllKeys(this IMemoryCache memoryCache)
        => memoryCache is not MSCache.MemoryCache cache ? [] : cache.Keys;
    
    /// <summary>
    /// Random return some keys
    /// </summary>
    public static IReadOnlyList<object> ScanKeys(this IMemoryCache memoryCache)
    {
        if (memoryCache is not MSCache.MemoryCache cache)
        {
            return [];
        }

        var keyCount = cache.Count;
        if (keyCount <= 0)
        {
            return [];
        }

        var random = new Random();
        var startIndex = random.Next(0, keyCount);
        const int maxKeyCount = 30;
        var maxIndex = Math.Min(startIndex + 1 + maxKeyCount, keyCount + 1);
        var endIndex = random.Next(startIndex + 1, maxIndex);

        return cache.Keys.Skip(startIndex).Take(endIndex - startIndex).ToList();
    }

    /// <summary>
    /// Remove all cache keys
    /// </summary>
    public static void ClearAll(this IMemoryCache memoryCache) => memoryCache.Clear(1.0);

    /// <summary>
    /// Remove at least the given percentage of the total entries (or estimated memory).
    /// </summary>
    /// <param name="percent">Valid value range [0,1], beyond the change range will be treated according to the upper/lower limit</param>
    public static void Clear(this IMemoryCache memoryCache, double percent)
    {
        if (percent <= 0)
        {
            return;
        }

        if (percent > 1)
        {
            percent = 1;
        }

        if (memoryCache is MSCache.MemoryCache cache)
        {
            cache.Compact(percent);
        }
    }
}

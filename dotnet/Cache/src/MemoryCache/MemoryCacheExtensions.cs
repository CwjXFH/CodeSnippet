using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;

namespace MemoryCache
{
    public static class MemoryCacheExtensions
    {
        public static int KeyCount(this IMemoryCache memoryCache)
        {
            if (memoryCache.GetType().GetProperty("Count")?.GetValue(memoryCache) is int count)
            {
                return count;
            }

            return -1;
        }

        /// <summary>
        /// Return all memory cache keys, and expired time if it exists.
        /// </summary>
        public static IDictionary<string, DateTimeOffset?> GetAllKeys(this IMemoryCache memoryCache)
        {
            var entriesCollection = memoryCache.GetType().GetProperty("EntriesCollection",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty)?.GetValue(memoryCache);


            if (entriesCollection is not IDictionary cacheDict)
            {
                return new Dictionary<string, DateTimeOffset?>(0);
            }

            var keyInfoDict = new Dictionary<string, DateTimeOffset?>();
            foreach (DictionaryEntry cache in cacheDict)
            {
                if (cache.Value is not ICacheEntry cacheEntry)
                {
                    continue;
                }

                if (cacheEntry.Key is string { Length: > 0 } cacheKey)
                {
                    keyInfoDict[cacheKey] = cacheEntry.AbsoluteExpiration;
                }
            }

            return keyInfoDict;
        }

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

            var compact = memoryCache.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                .Where(m => m.Name == "Compact")
                .FirstOrDefault(m => m.GetParameters().Length == 1);
            compact?.Invoke(memoryCache, new object[] { percent });
        }
    }
}
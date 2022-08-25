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
            var entriesCollection = memoryCache.GetType().GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty)?.GetValue(memoryCache);


            if (entriesCollection is not null and IDictionary cacheDict)
            {
                var keyInfoDict = new Dictionary<string, DateTimeOffset?>();
                foreach (DictionaryEntry cache in cacheDict)
                {
                    if (cache.Value is ICacheEntry cacheEntry)
                    {
                        if (cacheEntry.Key is string cachekey && string.IsNullOrEmpty(cachekey) == false)
                        {
                            keyInfoDict[cachekey] = cacheEntry.AbsoluteExpiration;
                        }
                    }
                }
                return keyInfoDict;
            }

            return new Dictionary<string, DateTimeOffset?>(0);
        }

        public static void ClearAll(this IMemoryCache memoryCache)
        {
            var compact = memoryCache.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                .Where(m => m.Name == "Compact")
                .FirstOrDefault(m => m.GetParameters().Length == 1);
            compact?.Invoke(memoryCache, new object[] { 1.0 });
        }
    }
}

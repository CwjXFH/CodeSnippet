using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;

namespace MemoryCache;

public static class MemoryCacheExtensions
{
    private static MemoryCacheVersion _cacheVersion = MemoryCacheVersion.None;

    /// <summary>
    /// Get the number of keys that are stored in the memory.
    /// </summary>
    /// <returns>-1 if failed, else the number of keys</returns>
    public static int KeyCount(this IMemoryCache memoryCache)
    {
        if (memoryCache.GetType().GetProperty("Count", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                ?.GetValue(memoryCache) is int count)
        {
            return count;
        }

        return -1;
    }

    /// <summary>
    /// Return all memory cache keys, and expired time if it exists.
    /// </summary>
    public static IDictionary<object, DateTimeOffset?> GetAllKeys(this IMemoryCache memoryCache)
    {
        var keyCount = memoryCache.KeyCount();
        return memoryCache.GetKeys(keyCount);
    }

    /// <summary>
    /// Return specified count memory cache keys, and expired time if it exists.
    /// </summary>
    public static IDictionary<object, DateTimeOffset?> GetKeys(this IMemoryCache memoryCache, int count)
    {
        if (count <= 0)
        {
            return new Dictionary<object, DateTimeOffset?>();
        }

        var instanceType = memoryCache.GetType();
        object? entriesCollection = null;
        var typeVersion = GetMemoryCacheVersion(instanceType);
        if (typeVersion == MemoryCacheVersion.V6)
        {
            entriesCollection = instanceType.GetProperty("EntriesCollection",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty)?.GetValue(memoryCache);
        }
        else if (typeVersion == MemoryCacheVersion.V7)
        {
            var coherentState = instanceType.GetField("_coherentState",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField)?.GetValue(memoryCache);
            entriesCollection = coherentState?.GetType().GetProperty("EntriesCollection",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty)?.GetValue(coherentState);
        }

        if (entriesCollection is not IDictionary cacheDict)
        {
            return new Dictionary<object, DateTimeOffset?>(0);
        }

        var keyInfoDict = new Dictionary<object, DateTimeOffset?>();
        foreach (DictionaryEntry cache in cacheDict)
        {
            if (cache.Value is not ICacheEntry cacheEntry)
            {
                continue;
            }

            keyInfoDict[cacheEntry.Key] = cacheEntry.AbsoluteExpiration;
            if (keyInfoDict.Keys.Count >= count)
            {
                break;
            }
        }

        return keyInfoDict;
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

        var compact = memoryCache.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
            .Where(m => m.Name == "Compact")
            .FirstOrDefault(m => m.GetParameters().Length == 1);
        compact?.Invoke(memoryCache, new object[] { percent });
    }

    private static MemoryCacheVersion GetMemoryCacheVersion(Type type)
    {
        if (_cacheVersion != MemoryCacheVersion.None)
        {
            return _cacheVersion;
        }

        var assemblyFullName = type.Assembly.FullName;
        if (string.IsNullOrWhiteSpace(assemblyFullName))
        {
            throw new ArgumentException(nameof(type));
        }

        _cacheVersion = assemblyFullName.Contains("Version=6.") ? MemoryCacheVersion.V6 :
            assemblyFullName.Contains("Version=7.") ? MemoryCacheVersion.V7 :
            throw new NotSupportedException();

        return _cacheVersion;
    }
}
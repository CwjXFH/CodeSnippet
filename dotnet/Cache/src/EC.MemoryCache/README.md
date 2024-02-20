## Summary

Extension methods for [MemoryCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.memorycache?view=dotnet-plat-ext-7.0), which implements [IMemoryCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.imemorycache?view=dotnet-plat-ext-7.0) interface.   

These extension methods used some internal members of MemoryCache, these members may be changed in different versions, so these extension does not promise to work for different MemoryCache.


## Release notes

Click [here](https://github.com/CwjXFH/CodeSnippet/blob/master/dotnet/Cache/src/EC.MemoryCache/RELEASE-NOTES.md) for release notes.


## Demo

```c#
public class CacheService(IMemoryCache memoryCache)
{
    public object? Get(string key) => memoryCache.TryGetValue(key, out var val) ? val : "null";

    public int Count() => memoryCache.KeyCount();

    public IDictionary<object, DateTimeOffset?> Keys() => memoryCache.GetAllKeys();

    public void Remove(string key) => memoryCache.Remove(key);

    public void Clear() => memoryCache.ClearAll();
}
```

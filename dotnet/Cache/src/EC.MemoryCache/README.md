## Summary

Extension methods for [MemoryCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.memorycache?view=dotnet-plat-ext-7.0), which implements [IMemoryCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.imemorycache?view=dotnet-plat-ext-7.0) interface.   

## Release notes

Click [here](https://github.com/CwjXFH/CodeSnippet/blob/master/dotnet/Cache/src/EC.MemoryCache/RELEASE-NOTES.md) for release notes.


## Demo

```c#
public class CacheService(IMemoryCache memoryCache)
{
    public int Count() => memoryCache.KeyCount();

    public void Clear() => memoryCache.ClearAll();
}
```

## Summary

Generic memory cache component, supports generic cache key.

## Release notes

Click [here](https://github.com/CwjXFH/CodeSnippet/blob/master/dotnet/Cache/src/EC.GenericMemoryCache/RELEASE-NOTES.md) for release notes.

## Demo

```csharp
using EC.GenericMemoryCache;
using Microsoft.Extensions.DependencyInjection;

using var serviceProvider = new ServiceCollection()
			.AddGenericMemoryCache()
			.BuildServiceProvider();

var cacheKey = new CacheKey { Value = 100 };
var cache = serviceProvider.GetRequiredService<IGenericMemoryCache<CacheKey, string>>();
cache.Set(cacheKey, "cacheValue", null);

var exists = cache.TryGetValue(cacheKey, out var cacheVal);

// true
Console.WriteLine(exists);	           
// cacheValue
Console.WriteLine(cacheVal);                   

readonly file record struct CacheKey
{
	public int Value { init; get; }
}
```

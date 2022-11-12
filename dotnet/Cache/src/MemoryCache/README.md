Extension methods for [MemoryCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.memorycache?view=dotnet-plat-ext-7.0), which implements [IMemoryCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.imemorycache?view=dotnet-plat-ext-7.0) interface.   

These extension methods used some internal members of MemoryCache, these members may be changed in different versions, so these extension does not promise to work for different MemoryCache.

This package has been tested on [Microsoft.Extensions.Caching.Memory](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory/7.0.0) v6.0.0+ and v7.0.0.

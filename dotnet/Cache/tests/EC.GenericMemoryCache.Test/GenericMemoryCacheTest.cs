using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace EC.GenericMemoryCache.Test;

public class GenericMemoryCacheTest(GenericMemoryCacheFixture fixture) : IClassFixture<GenericMemoryCacheFixture>
{
    [Fact]
    public void Set_StructKey_BeStored()
    {
        const string initCacheVal = "100";
        var cacheKey = new CacheKey { Value = 100 };
        var cache = fixture.Services.GetRequiredService<IGenericMemoryCache<CacheKey, string>>();

        var notExists = cache.TryGetValue(cacheKey, out _);
        cache.Set(cacheKey, initCacheVal, null);
        var exists = cache.TryGetValue(cacheKey, out var cacheVal);

        Assert.False(notExists);
        Assert.True(exists);
        Assert.Equal(initCacheVal, cacheVal);
    }

    [Fact]
    public void Get_NotExistKey_ReturnFalse()
    {
        var cacheKey = new CacheKey { Value = 100 };
        var cache = fixture.Services.GetRequiredService<IGenericMemoryCache<CacheKey, string>>();

        var exists = cache.TryGetValue(cacheKey, out _);

        Assert.False(exists);
    }

    [Fact]
    public async Task ScanExpire_DelExpireKey_ReturnTrue()
    {
        var cache = fixture.Services.GetRequiredService<IGenericMemoryCache<CacheKey, string>>();
        var cacheKey1 = new CacheKey { Value = 100 };
        cache.Set(cacheKey1, "100", TimeSpan.FromSeconds(10));
        var cacheKey2 = new CacheKey { Value = 200 };
        cache.Set(cacheKey2, "200", TimeSpan.FromMinutes(20));

        fixture.FakeTimeProvider.SetUtcNow(DateTimeOffset.UtcNow.AddMinutes(16));
        _ = cache.TryGetValue(cacheKey2, out _);
        await Task.Delay(100);
        var cache1Exists = cache.TryGetValue(cacheKey1, out _);
        var cache2Exists = cache.TryGetValue(cacheKey2, out _);

        Assert.False(cache1Exists);
        Assert.True(cache2Exists);
    }

    [Fact]
    public void Get_ExpireKey_ReturnFalse()
    {
        var cacheKey = new CacheKey { Value = 100 };
        var cache = fixture.Services.GetRequiredService<IGenericMemoryCache<CacheKey, string>>();
        cache.Set(cacheKey, "100", TimeSpan.FromSeconds(10));
        fixture.FakeTimeProvider.SetUtcNow(DateTimeOffset.UtcNow.AddSeconds(10));

        var exists = cache.TryGetValue(cacheKey, out _);

        Assert.False(exists);
    }

    [Fact]
    public void Remove_ExistKey_ReturnTrue()
    {
        const string initCacheVal = "100";
        var cacheKey = new CacheKey { Value = 100 };
        var cache = fixture.Services.GetRequiredService<IGenericMemoryCache<CacheKey, string>>();
        cache.Set(cacheKey, initCacheVal, null);

        var exists = cache.TryRemove(cacheKey, out var cacheVal);

        Assert.True(exists);
        Assert.Equal(initCacheVal, cacheVal);
    }

    [Fact]
    public void Remove_NotExistKey_ReturnFalse()
    {
        var cacheKey = new CacheKey { Value = 100 };
        var cache = fixture.Services.GetRequiredService<IGenericMemoryCache<CacheKey, string>>();

        var exists = cache.TryRemove(cacheKey, out var value);

        Assert.False(exists);
        Assert.Null(value);
    }
}

[SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly")]
public class GenericMemoryCacheFixture : IDisposable
{
    public readonly IServiceProvider Services;
    public readonly FakeTimeProvider FakeTimeProvider = new(DateTimeOffset.UtcNow);


    public GenericMemoryCacheFixture()
    {
        Services = new ServiceCollection()
            .AddGenericMemoryCache(opt => opt.TimeProvider = FakeTimeProvider)
            .BuildServiceProvider();
    }


    public void Dispose()
    {
        if (Services is ServiceProvider provider)
        {
            provider.Dispose();
        }
    }
}

readonly file record struct CacheKey
{
    public int Value { init; get; }
}

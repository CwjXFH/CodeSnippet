using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace EC.GenericMemoryCache.Test;

public class GenericMemoryCacheTest(GenericMemoryCacheFixture fixture) : IClassFixture<GenericMemoryCacheFixture>
{
    [Fact]
    public void Set_StructKey_ReturnTrue()
    {
        const string initCacheVal = "100";
        var cacheKey = new CacheKey { Value = 100 };
        var cache = fixture.Services.GetRequiredService<IGenericMemoryCache<CacheKey, string>>();
        cache.Set(cacheKey, initCacheVal, null);

        var exists = cache.TryGetValue(cacheKey, out var cacheVal);

        Assert.True(exists && cacheVal == initCacheVal);
    }

    [Fact]
    public void Expire_Key_ReturnTrue()
    {
        var cacheKey = new CacheKey { Value = 100 };
        var cache = fixture.Services.GetRequiredService<IGenericMemoryCache<CacheKey, string>>();
        cache.Set(cacheKey, "100", TimeSpan.FromSeconds(10));
        fixture.FakeTimeProvider.SetUtcNow(DateTimeOffset.Now.AddSeconds(10));

        var exists = cache.TryGetValue(cacheKey, out _);

        Assert.False(exists);
    }
}

[SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly")]
public class GenericMemoryCacheFixture : IDisposable
{
    public readonly IServiceProvider Services;
    public readonly FakeTimeProvider FakeTimeProvider = new();


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

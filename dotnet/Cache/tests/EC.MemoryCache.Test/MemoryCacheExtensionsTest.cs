using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace MemoryCache.Test;

public class MemoryCacheExtensionsTest
{
    private readonly IServiceProvider _serviceProvider = new ServiceCollection().AddMemoryCache().BuildServiceProvider();

    #region KeyCount

    [Fact]
    public void KeyCount_HasItems_ReturnTrue()
    {
        using var scope = _serviceProvider.CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        const int count = 3;
        for (var i = 0; i < count; i++)
        {
            memoryCache.Set($"{i}", i);
        }

        var keyCount = memoryCache.KeyCount();

        Assert.Equal(count, keyCount);
    }

    [Fact]
    public void KeyCount_Empty_ReturnTrue()
    {
        using var scope = _serviceProvider.CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

        var keyCount = memoryCache.KeyCount();

        Assert.Equal(0, keyCount);
    }

    [Fact]
    public void KeyCount_NotMemoryCache_ReturnMinusOne()
    {
        IMemoryCache memoryCache = new FakeMemoryCache();

        var keyCount = memoryCache.KeyCount();

        Assert.Equal(-1, keyCount);
    }

    #endregion

    #region GetAllKeys

    [Fact]
    public void GetAllKeys_HasItems_ReturnTrue()
    {
        using var scope = _serviceProvider.CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        const string key = "key";
        memoryCache.Set(key, 1);

        var keys = memoryCache.GetAllKeys();

        Assert.Equal(key, keys.First());
    }

    [Fact]
    public void GetAllKeys_Empty_ReturnTrue()
    {
        using var scope = _serviceProvider.CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

        var keys = memoryCache.GetAllKeys();

        Assert.Empty(keys);
    }

    [Fact]
    public void GetAllKeys_NotMemoryCache_ReturnEmpty()
    {
        IMemoryCache memoryCache = new FakeMemoryCache();

        var keys = memoryCache.GetAllKeys();

        Assert.Empty(keys);
    }

    #endregion

    #region ScanKeys

    [Fact]
    public void ScanKeys_Empty_ReturnTrue()
    {
        using var scope = _serviceProvider.CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

        var keys = memoryCache.ScanKeys();

        Assert.Empty(keys);
    }

    [Fact]
    public void ScanKeys_HasOneItem_ReturnTrue()
    {
        using var scope = _serviceProvider.CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        const string key = "key";
        memoryCache.Set(key, 1);

        var keys = memoryCache.ScanKeys();

        Assert.Equal(key, keys[0].ToString());
    }

    [Fact]
    public void ScanKeys_HasItems_ReturnTrue()
    {
        using var scope = _serviceProvider.CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        for (var i = 0; i < 10; i++)
        {
            memoryCache.Set(i, i);
        }

        var keys = memoryCache.ScanKeys();

        Assert.True(keys is { Count: > 0 });
    }

    [Fact]
    public void ScanKeys_NotMemoryCache_ReturnEmpty()
    {
        IMemoryCache memoryCache = new FakeMemoryCache();

        var keys = memoryCache.ScanKeys();

        Assert.Empty(keys);
    }

    #endregion

    #region Clear

    [Fact]
    public void ClearAll_HasItems_ReturnTrue()
    {
        using var scope = _serviceProvider.CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        memoryCache.Set("1", 1);

        memoryCache.ClearAll();
        var count = memoryCache.KeyCount();

        Assert.Equal(0, count);
    }

    [Fact]
    public void ClearAll_Empty_ReturnTrue()
    {
        using var scope = _serviceProvider.CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

        memoryCache.ClearAll();
        var count = memoryCache.KeyCount();

        Assert.Equal(0, count);
    }


    [Fact]
    public void Clear_PercentLessThanOrEqualZero_ReturnTrue()
    {
        using var scope = _serviceProvider.CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        memoryCache.Set("1", 1);

        memoryCache.Clear(0.0);
        var count = memoryCache.KeyCount();

        Assert.Equal(1, count);
    }

    [Fact]
    public void Clear_PercentGreaterThanOrEqualOne_ReturnTrue()
    {
        using var scope = _serviceProvider.CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        memoryCache.Set("1", 1);

        memoryCache.Clear(1.2);
        var count = memoryCache.KeyCount();

        Assert.Equal(0, count);
    }

    [Fact]
    public void Clear_PercentInRange_ReturnTrue()
    {
        using var scope = _serviceProvider.CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        const int keyCount = 10;
        for (var i = 0; i < keyCount; i++)
        {
            memoryCache.Set(i, i);
        }

        const double percent = 0.3;
        memoryCache.Clear(percent);
        var count = memoryCache.KeyCount();
        const double expected = keyCount * (1 - percent);

        Assert.True(expected >= count);
    }

    #endregion
}

sealed file class FakeMemoryCache : IMemoryCache
{
    public ICacheEntry CreateEntry(object key) => throw new NotImplementedException();

    public void Dispose() { }

    public void Remove(object key) => throw new NotImplementedException();

    public bool TryGetValue(object key, out object? value)
    {
        value = null;
        return false;
    }

    public MemoryCacheStatistics? GetCurrentStatistics() => null;
}

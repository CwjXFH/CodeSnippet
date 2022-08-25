using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace MemoryCache.Test
{
    public class MemoryCacheExtensionsTest
    {
        private IServiceProvider _serviceProvider;
        public MemoryCacheExtensionsTest()
        {
            _serviceProvider = new ServiceCollection().AddMemoryCache().BuildServiceProvider();
        }

        [Fact]
        public void KeyCount_HasItems_ReturnTrue()
        {
            using var scope = _serviceProvider.CreateScope();
            var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
            var count = 3;
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
        public void GetAllKeys_HasItems_ReturnTrue()
        {
            using var scope = _serviceProvider.CreateScope();
            var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
            var key = "key";
            memoryCache.Set(key, 1);

            var keys = memoryCache.GetAllKeys();

            Assert.Equal(key, keys.First().Key);
        }

        [Fact]
        public void GetAllKeys_Empty_ReturnTrue()
        {
            using var scope = _serviceProvider.CreateScope();
            var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

            var keys = memoryCache.GetAllKeys();

            Assert.Equal(0, keys.Count);
        }

        [Fact]
        public void GetAllKeys_HasItemWithoutExpire_ReturnTrue()
        {
            using var scope = _serviceProvider.CreateScope();
            var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
            var key = "key";
            memoryCache.Set(key, 1);

            var keys = memoryCache.GetAllKeys();

            Assert.Null(keys.First().Value);
        }

        [Fact]
        public void GetAllKeys_HasItemWithExpire_ReturnTrue()
        {
            using var scope = _serviceProvider.CreateScope();
            var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
            var key = "key";
            memoryCache.Set(key, 1, TimeSpan.FromSeconds(1));

            var keys = memoryCache.GetAllKeys();

            Assert.NotNull(keys.First().Value);
        }



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

    }
}

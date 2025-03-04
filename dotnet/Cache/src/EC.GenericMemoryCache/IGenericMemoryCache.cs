using System.Diagnostics.CodeAnalysis;

namespace EC.GenericMemoryCache;

public interface IGenericMemoryCache<TKey, TValue> where TKey : notnull
{
    void Set(TKey key, TValue value, TimeSpan? expire);
    bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
}

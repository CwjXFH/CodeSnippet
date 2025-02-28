namespace MemoryCache.Generic;

internal class CacheEntry<TValue>
{
    private readonly DateTime _createTime = DateTime.UtcNow;

    public TimeSpan? Expire { init; get; }
    public required TValue Value { init; get; }

    public bool Expired => Expire.HasValue && DateTime.UtcNow - _createTime > Expire.Value;
}

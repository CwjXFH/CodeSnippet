namespace EC.GenericMemoryCache;

internal class CacheEntry<TValue>
{
    private readonly DateTimeOffset _createTime = DateTimeOffset.UtcNow;

    public required TimeProvider TimeProvider { init; get; }

    public TimeSpan? Expire { init; get; }

    public required TValue Value { init; get; }

    public bool Expired => Expire.HasValue && TimeProvider.GetUtcNow() - _createTime > Expire.Value;
}

namespace EC.GenericMemoryCache;

internal class CacheEntry<TValue>
{
    private DateTimeOffset _createTime;

    public required TimeProvider TimeProvider
    {
        init
        {
            field = value;
            _createTime = field.GetUtcNow();
        }
        get;
    }

    public TimeSpan? Expire { init; get; }

    public required TValue Value { init; get; }

    public bool Expired => Expire.HasValue && TimeProvider.GetUtcNow() - _createTime > Expire.Value;
}

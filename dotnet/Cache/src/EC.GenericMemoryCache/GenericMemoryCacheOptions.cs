namespace EC.GenericMemoryCache;

public class GenericMemoryCacheOptions
{
    public TimeProvider TimeProvider { set; get; } = TimeProvider.System;

    public TimeSpan ExpirationScanFrequency { init; get; } = TimeSpan.FromMinutes(15);
}

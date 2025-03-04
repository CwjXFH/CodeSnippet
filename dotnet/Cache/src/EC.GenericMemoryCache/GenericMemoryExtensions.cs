using Microsoft.Extensions.DependencyInjection;

namespace EC.GenericMemoryCache;

public static class GenericMemoryExtensions
{
    public static IServiceCollection AddGenericMemoryCache(this IServiceCollection services)
    {
        services.AddOptions();
        services.AddSingleton(typeof(IGenericMemoryCache<,>), typeof(GenericMemoryCache<,>));

        return services;
    }

    public static IServiceCollection AddGenericMemoryCache(this IServiceCollection services, Action<GenericMemoryCacheOptions> options)
    {
        services.AddGenericMemoryCache().Configure(options);

        return services;
    }
}

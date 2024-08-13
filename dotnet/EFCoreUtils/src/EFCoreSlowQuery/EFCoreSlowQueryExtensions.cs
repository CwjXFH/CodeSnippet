/*
 * Implementation EFCore slow query trace based on .NET diagnostic functions.
 * 
 * References:
 * https://docs.microsoft.com/en-us/ef/core/logging-events-diagnostics/
 * https://github.com/dotnet/runtime/blob/main/src/libraries/System.Diagnostics.DiagnosticSource/src/DiagnosticSourceUsersGuide.md
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EFCoreSlowQuery
{
    /// <summary>
    /// Record EFCore slow query log
    /// </summary>
    public static class EFCoreSlowQueryExtensions
    {
        public static IApplicationBuilder UseEFCoreSlowQuery(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<IConfiguration>()?.GetSection(EFCoreSlowQueryOptions.OptionsName)
                .Get<EFCoreSlowQueryOptions>() ?? EFCoreSlowQueryOptions.DefaultOptions;

            var logger = app.CreateLogger();
            RegisterObserver(logger, options);

            return app;
        }

        public static IApplicationBuilder UseEFCoreSlowQuery(this IApplicationBuilder app, Action<EFCoreSlowQueryOptions> optionsAction)
        {
            if (optionsAction == null)
            {
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
                throw new ArgumentNullException();
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
            }
            var options = new EFCoreSlowQueryOptions();
            optionsAction.Invoke(options);

            var logger = app.CreateLogger();
            RegisterObserver(logger, options);

            return app;
        }

        private static ILogger CreateLogger(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            if (loggerFactory == null)
            {
#pragma warning disable S112 // General or reserved exceptions should never be thrown
                throw new NullReferenceException($"Cannot resolve {nameof(ILoggerFactory)} type from IoC container.");
#pragma warning restore S112 // General or reserved exceptions should never be thrown
            }
            var logger = loggerFactory.CreateLogger($"{nameof(EFCoreSlowQueryExtensions)}");

            return logger;
        }

        private static void RegisterObserver(ILogger logger, EFCoreSlowQueryOptions options)
        {
            var slowQueryObserver = new SlowQueryObserver(logger, options);
            DiagnosticListener.AllListeners.Subscribe(new EFCoreObserver(slowQueryObserver));
        }
    }
}

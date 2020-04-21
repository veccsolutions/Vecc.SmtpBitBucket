using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Vecc.SmtpBitBucket.Stores.Postgres;

namespace Vecc.SmtpBitBucket.Sandbox
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddCoreBitBucket();
            serviceCollection.AddSerilogLogging();
            //serviceCollection.AddInMemoryStores();
            serviceCollection.AddPostgresStores();
            serviceCollection.Configure<DatabaseOptions>((o) =>
            {
                o.ConnectionString = "Host=localhost;Port=6101;Username=postgres;Password=Abcd1234;Database=SmtpBitBucket;";
            });
            var provider = serviceCollection.BuildServiceProvider(true);

            return provider;
        }

        private static IServiceCollection AddSerilogLogging(this IServiceCollection serviceCollection)
        {
            var minimumLogLevel = Serilog.Events.LogEventLevel.Verbose;

            var serilogConfiguration = new LoggerConfiguration();

            serilogConfiguration.WriteTo.Console(minimumLogLevel, theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code);
            serilogConfiguration.WriteTo.Debug(minimumLogLevel);
            serilogConfiguration.MinimumLevel.Is(minimumLogLevel);

            var logger = serilogConfiguration.CreateLogger();

            serviceCollection.AddLogging(builder => builder.AddSerilog(logger));

            return serviceCollection;
        }
    }
}

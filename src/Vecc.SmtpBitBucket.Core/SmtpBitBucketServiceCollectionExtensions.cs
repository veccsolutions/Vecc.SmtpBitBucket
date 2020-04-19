using Vecc.SmtpBitBucket.Core.Providers;
using Vecc.SmtpBitBucket.Core.Providers.Internal;
using Vecc.SmtpBitBucket.Core.Server;
using Vecc.SmtpBitBucket.Core.Server.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmtpBitBucketServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreBitBucket(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();
            serviceCollection.AddSingleton<ISmtpConnectionFactory, DefaultSmtpConnectionFactory>();
            serviceCollection.AddSingleton<ISmtpServerFactory, DefaultSmtpServerFactory>();
            serviceCollection.AddSingleton<ISmtpHost, DefaultSmtpHost>();
            serviceCollection.AddSingleton<ServiceStore, ServiceStore>();
            serviceCollection.AddTransient<ISmtpServer, DefaultSmtpServer>();
            serviceCollection.Configure<ServerOptions>((o) => { }); //just use defaults for the server options
            return serviceCollection;
        }
    }
}

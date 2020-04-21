using Microsoft.Extensions.Configuration;
using Vecc.SmtpBitBucket.Core.Stores;
using Vecc.SmtpBitBucket.Stores.Postgres;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PostgresStoreServiceCollectionExtensions
    {
        public static IServiceCollection AddPostgresStores(this IServiceCollection services, IConfigurationSection databaseConfiguration)
        {
            services.AddPostgresStores();
            services.Configure<DatabaseOptions>(databaseConfiguration);

            return services;
        }

        public static IServiceCollection AddPostgresStores(this IServiceCollection services)
        {
            services.AddSingleton<IMessageStore, MessageStore>();
            services.AddSingleton<IRuleStore, RuleStore>();
            services.AddSingleton<ISessionStore, SessionStore>();
            services.AddSingleton<IUserStore, UserStore>();

            return services;
        }
    }
}

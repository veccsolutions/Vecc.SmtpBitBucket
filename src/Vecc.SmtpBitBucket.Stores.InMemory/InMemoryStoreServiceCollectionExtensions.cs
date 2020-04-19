using Vecc.SmtpBitBucket.Core.Stores;
using Vecc.SmtpBitBucket.Stores.InMemory;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InMemoryStoreServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryStores(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMessageStore, MessageStore>();
            serviceCollection.AddSingleton<IRuleStore, RuleStore>();
            serviceCollection.AddSingleton<ISessionStore, SessionStore>();
            serviceCollection.AddSingleton<IUserStore, UserStore>();

            return serviceCollection;
        }
    }
}

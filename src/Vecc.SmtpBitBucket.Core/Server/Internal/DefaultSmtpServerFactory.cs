using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Vecc.SmtpBitBucket.Core.Server.Internal
{
    public class DefaultSmtpServerFactory : ISmtpServerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultSmtpServerFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public Task<ISmtpServer> CreateTcpServerAsync()
        {
            var result = this._serviceProvider.GetRequiredService<ISmtpServer>();

            return Task.FromResult<ISmtpServer>(result);
        }
    }
}

using System;
using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core.Server.Internal
{
    public class DefaultSmtpConnectionFactory : ISmtpConnectionFactory
    {
        public Task<SmtpConnection> CreateSmtpConnectionAsync()
        {
            var result = new SmtpConnection
            {
                EhloHost = string.Empty,
                Id = Guid.NewGuid(),
                MailFrom = string.Empty,
            };

            return Task.FromResult(result);
        }
    }
}

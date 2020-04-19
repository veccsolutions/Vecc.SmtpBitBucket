using System.Net;
using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core.Server.Internal
{
    public class DefaultSmtpConfiguration : ISmtpConfiguration
    {
        public Task<string> GetWelcomeMessageAsync() => Task.FromResult(Dns.GetHostName());
    }
}

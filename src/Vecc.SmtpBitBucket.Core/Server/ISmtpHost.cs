using System.Threading;
using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core.Server
{
    public interface ISmtpHost
    {
        Task RunAsync(int port, CancellationToken token);
    }
}

using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core.Server
{
    public interface ISmtpServer
    {
        Task StartAsync(TcpClient client, CancellationToken hangupToken);
    }
}

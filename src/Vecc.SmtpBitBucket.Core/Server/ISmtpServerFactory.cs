using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core.Server
{
    public interface ISmtpServerFactory
    {
        Task<ISmtpServer> CreateTcpServerAsync();
    }
}

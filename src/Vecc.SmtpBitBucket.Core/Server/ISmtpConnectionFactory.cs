using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core.Server
{
    public interface ISmtpConnectionFactory
    {
        Task<SmtpConnection> CreateSmtpConnectionAsync();
    }
}

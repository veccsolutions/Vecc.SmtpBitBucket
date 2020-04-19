using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core.Server
{
    public interface ISmtpConfiguration
    {
        Task<string> GetWelcomeMessageAsync();
    }
}

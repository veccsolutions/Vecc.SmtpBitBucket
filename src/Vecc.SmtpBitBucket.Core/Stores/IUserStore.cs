using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core.Stores
{
    public interface IUserStore
    {
        Task<bool> IsUserEnabledAsync(string username);
        Task DisableUserAsync(string username);
        Task CreateUserAsync(string username, string password);
        Task<bool> IsValidAsync(string username, string password);
    }
}

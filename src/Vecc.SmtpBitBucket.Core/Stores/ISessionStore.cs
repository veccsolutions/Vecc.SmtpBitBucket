using System;
using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core.Stores
{
    public interface ISessionStore
    {
        Task<SmtpSession> GetSessionByIdAsync(int id);
        Task<SmtpSession> StoreSessionAsync(SmtpSession session);
        Task AddChatterAsync(int sessionId, string message, Direction direction);
        Task<SmtpSession[]> GetSessionsAsync(DateTime? oldestDate = null, DateTime? newestDate = null, string username = null);
        Task<SmtpSessionChatter[]> GetSessionChatterAsync(int sessionId);
        Task UpdateSessionEndTime(int sessionId, DateTime endTime);
        Task UpdateSessionUsername(int sessionId, string username);
    }
}

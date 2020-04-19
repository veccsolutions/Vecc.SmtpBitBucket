using System;
using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core.Stores
{
    public interface ISessionStore
    {
        Task<SmtpSession> GetSessionByIdAsync(Guid id);
        Task StoreSessionAsync(SmtpSession session);
        Task AddChatterAsync(Guid sessionId, string message, Direction direction);
        Task<SmtpSession[]> GetSessionsAsync(DateTime? oldestDate = null, DateTime? newestDate = null, string username = null);
        Task<SmtpSessionChatter[]> GetSessionChatterAsync(Guid sessionId);
        Task UpdateSessionEndTime(Guid sessionId, DateTime endTime);
        Task UpdateSessionUsername(Guid sessionId, string username);
    }
}

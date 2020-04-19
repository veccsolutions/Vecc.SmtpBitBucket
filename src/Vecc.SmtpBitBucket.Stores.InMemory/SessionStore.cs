using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vecc.SmtpBitBucket.Core;
using Vecc.SmtpBitBucket.Core.Providers;
using Vecc.SmtpBitBucket.Core.Stores;

namespace Vecc.SmtpBitBucket.Stores.InMemory
{
    public class SessionStore : ISessionStore
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly List<Chatter> _chatter = new List<Chatter>();
        private readonly List<SmtpSession> _sessions = new List<SmtpSession>();

        public SessionStore(IDateTimeProvider dateTimeProvider)
        {
            this._dateTimeProvider = dateTimeProvider;
        }

        public Task AddChatterAsync(Guid sessionId, string message, Direction direction)
        {
            this._chatter.Add(new Chatter
            {
                SessionId = sessionId,
                Data = new SmtpSessionChatter
                {
                    Data = message,
                    Direction = direction,
                    Timestamp = this._dateTimeProvider.UtcNow
                }
            });

            return Task.CompletedTask;
        }

        public Task<SmtpSession> GetSessionByIdAsync(Guid id) => Task.FromResult(this._sessions.FirstOrDefault(x => x.SessionId == id));

        public Task<SmtpSessionChatter[]> GetSessionChatterAsync(Guid sessionId) => Task.FromResult(this._chatter.Where(x => x.SessionId == sessionId).Select(x => x.Data).ToArray());

        public Task<SmtpSession[]> GetSessionsAsync(DateTime? oldestDate = null, DateTime? newestDate = null, string username = null) => Task.FromResult(this._sessions.Where(x =>
                (x.SessionStartTime >= (oldestDate ?? DateTime.MinValue)) &&
                (x.SessionStartTime <= (newestDate ?? DateTime.MaxValue)) &&
                (string.IsNullOrWhiteSpace(username) || x.Username == username))
            .ToArray());

        public Task StoreSessionAsync(SmtpSession session)
        {
            this._sessions.Add(session);

            return Task.CompletedTask;
        }

        public async Task UpdateSessionEndTime(Guid sessionId, DateTime endTime)
        {
            var session = await this.GetSessionByIdAsync(sessionId);
            if (session != null)
            {
                session.SessionEndTime = endTime;
            }
        }

        public async Task UpdateSessionUsername(Guid sessionId, string username)
        {
            var session = await this.GetSessionByIdAsync(sessionId);
            if (session != null)
            {
                session.Username = username;
            }
        }

    }
}

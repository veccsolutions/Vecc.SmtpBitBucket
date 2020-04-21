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

        public Task AddChatterAsync(int sessionId, string message, Direction direction)
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

        public Task<SmtpSession> GetSessionByIdAsync(int id) => Task.FromResult(this._sessions.FirstOrDefault(x => x.SessionId == id));

        public Task<SmtpSessionChatter[]> GetSessionChatterAsync(int sessionId) => Task.FromResult(this._chatter.Where(x => x.SessionId == sessionId).Select(x => x.Data).ToArray());

        public Task<SmtpSession[]> GetSessionsAsync(DateTime? oldestDate = null, DateTime? newestDate = null, string username = null) => Task.FromResult(this._sessions.Where(x =>
                (x.SessionStartTime >= (oldestDate ?? DateTime.MinValue)) &&
                (x.SessionStartTime <= (newestDate ?? DateTime.MaxValue)) &&
                (string.IsNullOrWhiteSpace(username) || x.Username == username))
            .ToArray());

        public Task<SmtpSession> StoreSessionAsync(SmtpSession session)
        {
            var sessionId = 1;

            if (this._sessions.Count > 0)
            {
                sessionId = this._sessions.Max(x => x.SessionId) + 1;
            }

            session.SessionId = sessionId;

            this._sessions.Add(session);

            return Task.FromResult(session);
        }

        public async Task UpdateSessionEndTime(int sessionId, DateTime endTime)
        {
            var session = await this.GetSessionByIdAsync(sessionId);
            if (session != null)
            {
                session.SessionEndTime = endTime;
            }
        }

        public async Task UpdateSessionUsername(int sessionId, string username)
        {
            var session = await this.GetSessionByIdAsync(sessionId);
            if (session != null)
            {
                session.Username = username;
            }
        }

    }
}

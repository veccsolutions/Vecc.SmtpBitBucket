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
            this.InitializeSessions();
            this.InitializeChatter();
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

        private void InitializeSessions()
        {
            var now = this._dateTimeProvider.UtcNow;
            this._sessions.Add(new SmtpSession
            {
                RemoteIp = "1.1.1.1",
                SessionEndTime = now.AddSeconds(-1),
                SessionId = 2,
                SessionStartTime = now.AddSeconds(-1).AddMilliseconds(9),
                Username = null
            });

            this._sessions.Add(new SmtpSession
            {
                RemoteIp = "127.0.0.1",
                SessionEndTime = now.AddDays(-8),
                SessionId = 1,
                SessionStartTime = now.AddDays(-8).AddSeconds(1),
                Username = null
            });
        }

        private void InitializeChatter()
        {
            this.AddChatter(1, "220 Hi", Direction.Out, 0);
            this.AddChatter(1, "HELO me", Direction.In, 0);
            this.AddChatter(1, "250 OK", Direction.Out, 0);
            this.AddChatter(1, "MAIL FROM:<test@test.com>", Direction.In, 0);
            this.AddChatter(1, "250 OK", Direction.Out, 0);
            this.AddChatter(1, "RCPT TO:<test1@test1.com>", Direction.In, 0);
            this.AddChatter(1, "250 OK", Direction.Out, 0);
            this.AddChatter(1, "DATA", Direction.In, 0);
            this.AddChatter(1, "354 End data with <CR><LF>.<CR><LF>", Direction.Out, 0);
            this.AddChatter(1, "From: \"Test\" <test@test.com>", Direction.In, 1);
            this.AddChatter(1, "To: \"Test1\" <test1@test1.com", Direction.In, 1);
            this.AddChatter(1, "Subject: Test Message", Direction.In, 1);
            this.AddChatter(1, "", Direction.In, 1);
            this.AddChatter(1, "Hello Test", Direction.In, 1);
            this.AddChatter(1, "This is a test message with 4 header fields and 4 lines in the message body.", Direction.In, 2);
            this.AddChatter(1, "Your friend,", Direction.In, 3);
            this.AddChatter(1, "Bob", Direction.In, 4);
            this.AddChatter(1, ".", Direction.In, 5);
            this.AddChatter(1, "250 Ok: queued as 12345", Direction.Out, 6);
            this.AddChatter(1, "QUIT", Direction.In, 7);
            this.AddChatter(1, "221 Bye", Direction.In, 8);


            this.AddChatter(2, "220 Hi", Direction.Out, 0);
            this.AddChatter(2, "HELO me", Direction.In, 0);
            this.AddChatter(2, "250 OK", Direction.Out, 0);
            this.AddChatter(2, "MAIL FROM:<test@test.com>", Direction.In, 0);
            this.AddChatter(2, "250 OK", Direction.Out, 0);
            this.AddChatter(2, "RCPT TO:<test1@test1.com>", Direction.In, 0);
            this.AddChatter(2, "250 OK", Direction.Out, 0);
            this.AddChatter(2, "DATA", Direction.In, 0);
            this.AddChatter(2, "354 End data with <CR><LF>.<CR><LF>", Direction.Out, 0);
            this.AddChatter(2, "From: \"Test\" <test@test.com>", Direction.In, 1);
            this.AddChatter(2, "To: \"Test1\" <test1@test1.com", Direction.In, 1);
            this.AddChatter(2, "Subject: Test Message", Direction.In, 1);
            this.AddChatter(2, "", Direction.In, 1);
            this.AddChatter(2, "Hello Test", Direction.In, 1);
            this.AddChatter(2, "This is a test message with 4 header fields and 4 lines in the message body.", Direction.In, 2);
            this.AddChatter(2, "Your friend,", Direction.In, 3);
            this.AddChatter(2, "Bob", Direction.In, 4);
            this.AddChatter(2, ".", Direction.In, 5);
            this.AddChatter(2, "250 Ok: queued as 12345", Direction.Out, 6);
            this.AddChatter(2, "QUIT", Direction.In, 7);
            this.AddChatter(2, "221 Bye", Direction.In, 8);
        }
        private Chatter AddChatter(int sessionId, string data, Direction direction, int msOffset)
        {
            var result = new Chatter
            {
                Data = new SmtpSessionChatter
                {
                    Data = data,
                    Direction = direction,
                    Timestamp = this._sessions[sessionId - 1].SessionStartTime.Value.AddMilliseconds(msOffset)
                 },
                 SessionId = sessionId
            };

            this._chatter.Add(result);
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Vecc.SmtpBitBucket.Core;
using Vecc.SmtpBitBucket.Core.Providers;
using Vecc.SmtpBitBucket.Core.Stores;
using Vecc.SmtpBitBucket.Stores.Postgres.Models;

namespace Vecc.SmtpBitBucket.Stores.Postgres
{
    public class SessionStore : Database, ISessionStore
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public SessionStore(IOptions<DatabaseOptions> databaseOptions, IDateTimeProvider dateTimeProvider) : base(databaseOptions)
        {
            this._dateTimeProvider = dateTimeProvider;
        }

        public async Task AddChatterAsync(int sessionId, string message, Direction direction)
        {
            using (var connection = this.GetConnection())
            {
                await connection.ExecuteAsync("INSERT INTO \"Chatter\" (\"SessionId\", \"Direction\", \"Timestamp\", \"Data\") VALUES (@sessionId, @direction, @timestamp, @data)",
                    new
                    {
                        sessionId = sessionId,
                        direction = direction.ToString(),
                        timestamp = this._dateTimeProvider.UtcNow,
                        data = message
                    });
            }
        }
        public async Task<SmtpSession> GetSessionByIdAsync(int id)
        {
            using (var connection = this.GetConnection())
            {
                var session = await connection.QuerySingleOrDefaultAsync<DataSession>("SELECT * FROM Sessions WHERE Id = @id", new { id = id });
                if (session != null)
                {
                    var result = new SmtpSession
                    {
                        RemoteIp = session.RemoteIP,
                        SessionEndTime = session.SessionEndTime,
                        SessionId = session.Id,
                        SessionStartTime = session.SessionStartTime,
                        Username = session.Username
                    };

                    return result;
                }

                return null;
            }
        }

        public async Task<SmtpSessionChatter[]> GetSessionChatterAsync(int sessionId)
        {
            var result = new List<SmtpSessionChatter>();

            using (var connection = this.GetConnection())
            {
                var dataChatter = await connection.QueryAsync<DataChatter>("SELECT * FROM \"Chatter\" WHERE \"SessionId\" = @sessionId ORDER BY \"Id\"", new { sessionId = sessionId });
                foreach (var chatter in dataChatter)
                {
                    Enum.TryParse<Direction>(chatter.Direction, true, out var direction);

                    var chat = new SmtpSessionChatter
                    {
                        Data = chatter.Data,
                        Direction = direction,
                        Timestamp = chatter.Timestamp
                    };

                    result.Add(chat);
                }
            }

            return result.ToArray();
        }

        public async Task<SmtpSession[]> GetSessionsAsync(DateTime? oldestDate = null, DateTime? newestDate = null, string username = null)
        {
            var result = new List<SmtpSession>();
            var query = "SELECT * FROM \"Sessions\"";
            if (oldestDate.HasValue || newestDate.HasValue || !string.IsNullOrWhiteSpace(username))
            {
                query += " WHERE ";
                var predicates = new List<string>();

                if (oldestDate.HasValue)
                {
                    predicates.Add("\"SessionStartTime\" >= @oldestTime");
                }

                if (newestDate.HasValue)
                {
                    predicates.Add("\"SessionStartTime\" <= @newestTime");
                }

                if (!string.IsNullOrWhiteSpace(username))
                {
                    predicates.Add("\"Username\" LIKE @username");
                }
            }

            using (var connection = this.GetConnection())
            {
                var dataSessions = await connection.QueryAsync<DataSession>(query, new { oldestTime = oldestDate, newestTime = newestDate, username = username });
                foreach (var dataSession in dataSessions)
                {
                    var session = new SmtpSession
                    {
                        RemoteIp = dataSession.RemoteIP,
                        SessionEndTime = dataSession.SessionEndTime,
                        SessionId = dataSession.Id,
                        SessionStartTime = dataSession.SessionStartTime,
                        Username = dataSession.Username
                    };

                    result.Add(session);
                }
            }

            return result.ToArray();
        }

        public Task<SmtpSession> StoreSessionAsync(SmtpSession session)
        {
            if (session.SessionId == 0)
            {
                return this.InsertSessionAsync(session);
            }

            return this.UpdateSessionAsync(session);
        }

        public async Task UpdateSessionEndTime(int sessionId, DateTime endTime)
        {
            using (var connection = this.GetConnection())
            {
                var query = @"
UPDATE ""Sessions""
SET ""SessionEndTime"" = @sessionEndTime
WHERE ""Id"" = @id";
                await connection.ExecuteAsync(query, new { id = sessionId, sessionEndTime = endTime });
            }
        }
        public async Task UpdateSessionUsername(int sessionId, string username)
        {
            using (var connection = this.GetConnection())
            {
                var query = @"
UPDATE ""Sessions""
SET ""Username"" = @username
WHERE ""Id"" = @id";
                await connection.ExecuteAsync(query, new { id = sessionId, username = username });
            }
        }

        private async Task<SmtpSession> InsertSessionAsync(SmtpSession session)
        {
            using (var connection = this.GetConnection())
            {
                var query = @"
INSERT INTO ""public"".""Sessions""
(""SessionStartTime"", ""SessionEndTime"", ""RemoteIP"", ""Username"")
VALUES
(@sessionStartTime, @sessionEndTime, @remoteIP, @username)
RETURNING ""Id"";
";
                var id = await connection.ExecuteScalarAsync<int>(query, new
                {
                     sessionStartTime = session.SessionStartTime,
                     sessionEndTime = session.SessionEndTime,
                     remoteIp = session.RemoteIp,
                     username = session.Username
                });
                session.SessionId = id;

                return session;
            }
        }

        private async Task<SmtpSession> UpdateSessionAsync(SmtpSession session)
        {
            using (var connection = this.GetConnection())
            {
                var query = @"
UPDATE Sessions
SET
    SessionStartTime = @sessionStartTime,
    SessionEndTime = @sessionEndTime,
    RemoteIP = @remoteIP,
    Username = @username
WHERE Id = @id";

                await connection.ExecuteAsync(query, new
                {
                    sessionStartTime = session.SessionStartTime,
                    sessionEndTime = session.SessionEndTime,
                    remoteIp = session.RemoteIp,
                    username = session.Username
                });

                return session;
            }
        }
    }
}

using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Vecc.SmtpBitBucket.Core;
using Vecc.SmtpBitBucket.Core.Stores;

namespace Vecc.SmtpBitBucket.Stores.Postgres
{
    public class MessageStore : Database, IMessageStore
    {
        public MessageStore(IOptions<DatabaseOptions> databaseOptions) : base(databaseOptions)
        {
        }

        public async Task<MailMessage> GetMessageByIdAsync(int id)
        {
            using (var connection = this.GetConnection())
            {
                var message = await connection.ExecuteScalarAsync<string>("SELECT \"Data\" FROM \"MailMessages\" WHERE \"Id\" = @id;", new { id = id });

                if (!string.IsNullOrWhiteSpace(message))
                {
                    var result = JsonSerializer.Deserialize<MailMessage>(message);
                    return result;
                }

                return null;
            }
        }

        public Task<MailMessage> StoreMessageAsync(MailMessage message)
        {
            if (message.Id != 0)
            {
                return this.UpdateMessageAsync(message);
            }

            return this.InsertMessageAsync(message);
        }

        private async Task<MailMessage> InsertMessageAsync(MailMessage message)
        {
            using (var connection = this.GetConnection())
            {
                var data = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });
                var id = await connection.ExecuteScalarAsync<int>(@"
INSERT INTO ""MailMessages""
(""SessionId"", ""ClientEhlo"", ""MailFrom"", ""MailTo"", ""Subject"", ""ReceivedAt"", ""Username"", ""Data"")
VALUES
(@sessionId, @clientEhlo, @mailFrom, @mailTo, @subject, @receivedAt, @username, @data)
RETURNING ""Id"";
",
                    new
                    {
                        sessionId = message.SessionId,
                        clientEhlo = message.ClientEhlo,
                        mailFrom = message.MailFrom,
                        mailTo = string.Join(",", message.Receipients),
                        receivedAt = message.Timestamp,
                        subject = message.Subject,
                        username = message.Username,
                        data = data
                    });

                message.Id = id;
            }

            return message;
        }

        private async Task<MailMessage> UpdateMessageAsync(MailMessage message)
        {
            using (var connection = this.GetConnection())
            {
                var data = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });
                await connection.ExecuteAsync(@"
UPDATE ""MailMessages""
SET 
    ""SessionId"" = @sessionId,
    ""ClientEhlo"" = @clientEhlo,
    ""MailFrom"" = @mailFrom,
    ""ReceivedAt"" = @receivedAt,
    ""Username"" = @username,
    ""Data"" = @data
WHERE ""Id"" = @id;
",
                    new
                    {
                        sessionId = message.SessionId,
                        clientEhlo = message.ClientEhlo,
                        mailFrom = message.MailFrom,
                        receivedAt = message.Timestamp,
                        username = message.Username,
                        data = data,
                        id = message.Id
                    });
            }

            return message;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vecc.SmtpBitBucket.Core;
using Vecc.SmtpBitBucket.Core.Stores;

namespace Vecc.SmtpBitBucket.Stores.InMemory
{
    public class MessageStore : IMessageStore
    {
        private readonly List<MailMessage> _store = new List<MailMessage>();

        public Task<MailMessage> GetMessageByIdAsync(int id) => Task.FromResult(this._store.FirstOrDefault(x => x.Id == id));

        public Task<MessageSummary[]> GetMessageSummariesAsync(string username)
        {
            var summaries = this._store.Where(x => username == null || x.Username == username)
                .Select(x => new MessageSummary
                {
                    ClientEhlo = x.ClientEhlo,
                    Id = x.Id,
                    MailFrom = x.MailFrom,
                    MailTo = string.Join(", ", x.Receipients),
                    ReceiveAt = x.Timestamp,
                    SessionId = x.SessionId,
                    Subject = x.Subject,
                    Username = x.Username
                })
                .ToArray();

            return Task.FromResult(summaries);
        }

        public Task<MailMessage> StoreMessageAsync(MailMessage message)
        {
            if (message.Id == 0)
            {
                if (this._store.Count > 0)
                {
                    message.Id = this._store.Max(x => x.Id) + 1;
                }
                message.Id = 1;
            }
            this._store.Add(message);

            return Task.FromResult(message);
        }
    }
}

using System.Linq;
using Vecc.SmtpBitBucket.Core;
using Vecc.SmtpBitBucket.Site.Api.V1.Models;

namespace Vecc.SmtpBitBucket.Site.Api.V1.Services.Internal
{
    public class DefaultModelConverter : IModelConverter
    {
        private readonly IDataEncryptor _dataEncryptor;

        public DefaultModelConverter(IDataEncryptor dataEncryptor)
        {
            this._dataEncryptor = dataEncryptor;
        }

        public Models.MessageSummary ToMessageSummary(Core.MessageSummary messageSummary)
        {
            if (messageSummary == null)
            {
                return null;
            }

            var result = new Models.MessageSummary
            {
                From = messageSummary.MailFrom,
                Id = messageSummary.Id,
                MessageId = this._dataEncryptor.EncryptMessageId(messageSummary.Id),
                Subject = messageSummary.Subject,
                TimeStamp = messageSummary.ReceiveAt,
                To = messageSummary.MailTo
            };

            return result;
        }

        public MessageSummaries ToMessageSummaries(Core.MessageSummary[] messages)
        {
            if (messages == null)
            {
                return null;
            }

            var messageSummaries = messages.Select(this.ToMessageSummary).ToArray();
            var result = new MessageSummaries
            {
                Messages = messageSummaries
            };

            return result;
        }

        public SessionDetails GetSessionDetails(SmtpSession session, SmtpSessionChatter[] sessionChatter, Core.MessageSummary[] messageSummaries)
        {
            if (session == null)
            {
                return null;
            }

            var result = new SessionDetails
            {
                Id = session.SessionId,
                MessageSummaries = this.ToMessageSummaries(messageSummaries),
                RemoteIp = session.RemoteIp,
                SessionChatter = this.GetSessionChatter(sessionChatter),
                SessionEndTime = session.SessionEndTime,
                SessionId = this._dataEncryptor.EncryptSessionId(session.SessionId),
                SessionStartTime = session.SessionStartTime,
                Username = session.Username
            };

            return result;
        }

        public SessionSummary ToSessionSummary(SmtpSession core)
        {
            if (core == null)
            {
                return null;
            }

            var result = new SessionSummary
            {
                Id = core.SessionId,
                RemoteIp = core.RemoteIp,
                SessionEndTime = core.SessionEndTime,
                SessionId = this._dataEncryptor.EncryptSessionId(core.SessionId),
                SessionStartTime = core.SessionStartTime,
                Username = core.Username
            };

            return result;
        }

        private SessionChatter GetSessionChatter(SmtpSessionChatter sessionChatter)
        {
            if (sessionChatter == null)
            {
                return null;
            }

            var result = new SessionChatter
            {
                Data = sessionChatter.Data,
                Direction = sessionChatter.Direction == Direction.In ? ChatterDirection.ToClient : ChatterDirection.ToServer,
                TimeStamp = sessionChatter.Timestamp
            };

            return result;
        }

        private SessionChatter[] GetSessionChatter(SmtpSessionChatter[] sessionChatter)
        {
            if (sessionChatter == null)
            {
                return null;
            }

            var result = sessionChatter.Select(this.GetSessionChatter).ToArray();
            return result;
        }
    }
}

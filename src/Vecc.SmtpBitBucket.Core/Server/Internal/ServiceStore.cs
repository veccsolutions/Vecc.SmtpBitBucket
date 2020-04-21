using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vecc.SmtpBitBucket.Core.Events;
using Vecc.SmtpBitBucket.Core.Providers;
using Vecc.SmtpBitBucket.Core.Stores;

namespace Vecc.SmtpBitBucket.Core.Server.Internal
{
    public class ServiceStore
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<ServiceStore> _logger;
        //private readonly IMessageNotifier _messageNotifier;
        private readonly IMessageStore _messageStore;
        private readonly ISessionStore _sessionStore;

        public ServiceStore(
            IDateTimeProvider dateTimeProvider,
            ILogger<ServiceStore> logger,
            //IMessageNotifier messageNotifier,
            IMessageStore messageStore,
            ISessionStore sessionStore)
        {
            this._dateTimeProvider = dateTimeProvider;
            this._logger = logger;
            //this._messageNotifier = messageNotifier;
            this._messageStore = messageStore;
            this._sessionStore = sessionStore;
        }

        internal async Task<SmtpConnection> CreateNewSessionAsync(string remoteIp)
        {
            var smtpSession = new SmtpConnection
            {
                EhloHost = string.Empty,
                MailFrom = string.Empty,
            };

            var session = await this._sessionStore.StoreSessionAsync(new SmtpSession
            {
                RemoteIp = remoteIp,
                SessionStartTime = this._dateTimeProvider.UtcNow,
                SessionEndTime = null
            });

            smtpSession.Id = session.SessionId;

            return smtpSession;
        }

        internal Task UpdateSessionUsernameAsync(SmtpConnection session) => this._sessionStore.UpdateSessionUsername(session.Id, session.Username);

        internal Task StoreChatterAsync(string message, Direction direction, int sessionId) => this._sessionStore.AddChatterAsync(sessionId, message, this.ToCore(direction));

        private Core.Direction ToCore(Direction service)
        {
            switch (service)
            {
                case Direction.In:
                    return SmtpBitBucket.Core.Direction.In;
                case Direction.Out:
                    return SmtpBitBucket.Core.Direction.Out;
            }

            throw new ArgumentOutOfRangeException("service");
        }

        internal async Task StoreMessageAsync(SmtpConnection session)
        {
            var message = new Core.MailMessage
            {
                ClientEhlo = session.EhloHost,
                Data = string.Join(Environment.NewLine, session.Data),
                MailFrom = session.MailFrom,
                Receipients = session.Recipients.ToArray(),
                SessionId = session.Id,
                Subject = session.Subject,
                Timestamp = DateTime.UtcNow,
                Username = session.Username,
                UseUtf8 = session.UseUtf8
            };

            message = await this._messageStore.StoreMessageAsync(message);

            if (message != null)
            {
                //this._messageNotifier.OnMessageReceived(session.Id, message.Id, this._dateTimeProvider.UtcNow);
            }
        }

        internal Task EndSessionAsync(SmtpConnection session) => this._sessionStore.UpdateSessionEndTime(session.Id, this._dateTimeProvider.UtcNow);
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vecc.SmtpBitBucket.Core.Providers;
using Vecc.SmtpBitBucket.Core.Stores;
using Vecc.SmtpBitBucket.Site.Api.V1.Models;
using Vecc.SmtpBitBucket.Site.Api.V1.Services;

namespace Vecc.SmtpBitBucket.Site.Api.V1.Controllers
{
    [Route("api/v1/sessions")]
    [ApiController]
    public class SessionsController : Controller
    {
        private readonly ILogger<SessionsController> _logger;
        private readonly IDataEncryptor _dataEncryptor;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMessageStore _messageStore;
        private readonly IModelConverter _modelConverter;
        private readonly ISessionStore _sessionStore;

        public SessionsController(ILogger<SessionsController> logger,
            IDataEncryptor dataEncryptor,
            IDateTimeProvider dateTimeProvider,
            IMessageStore messageStore,
            IModelConverter modelConverter,
            ISessionStore sessionStore)
        {
            this._logger = logger;
            this._dataEncryptor = dataEncryptor;
            this._dateTimeProvider = dateTimeProvider;
            this._messageStore = messageStore;
            this._modelConverter = modelConverter;
            this._sessionStore = sessionStore;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<SessionSummaries>> GetSummaries([FromQuery] DateTime? start, [FromQuery] DateTime? end, [FromQuery] string username)
        {
            if (start == null)
            {
                start = this._dateTimeProvider.UtcNow.AddDays(-1);
            }

            if (end == null)
            {
                end = this._dateTimeProvider.UtcNow;
            }

            var sessions = await this._sessionStore.GetSessionsAsync(start, end, username);
            var summaries = sessions.Select(this._modelConverter.ToSessionSummary).ToArray();
            var result = new SessionSummaries
            {
                Sessions = summaries
            };

            return result;
        }

        [HttpGet("details")]
        public async Task<ActionResult<SessionDetails>> GetDetails([FromQuery] [Required(AllowEmptyStrings = false)] string id)
        {
            if (!this.ModelState.IsValid)
            {
                this._logger.LogError("Invalid model state: {@modelState}", this.ModelState);
                return this.BadRequest();
            }

            if (!this._dataEncryptor.TryDecryptSessionId(id, out var decryptedId))
            {
                this._logger.LogError("Unable to decrypt id: {@id}", id);
                return this.BadRequest();
            }

            if (decryptedId <= 0)
            {
                this._logger.LogError("Decrypted id was less than or equal to 0: {@id}", decryptedId);
                return this.BadRequest();
            }

            var session = await this._sessionStore.GetSessionByIdAsync(decryptedId);
            var sessionChatter = await this._sessionStore.GetSessionChatterAsync(session.SessionId);
            var messageSummaries = await this._messageStore.GetMessageSummariesBySessionIdAsync(session.SessionId);
            var result = this._modelConverter.GetSessionDetails(session, sessionChatter, messageSummaries);

            return result;
        }
    }
}

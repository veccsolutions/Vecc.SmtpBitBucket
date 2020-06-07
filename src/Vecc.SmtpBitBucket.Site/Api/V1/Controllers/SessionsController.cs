using System;
using System.Collections.Generic;
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
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IModelConverter _modelConverter;
        private readonly ISessionStore _sessionStore;

        public SessionsController(ILogger<SessionsController> logger, IDateTimeProvider dateTimeProvider, IModelConverter modelConverter, ISessionStore sessionStore)
        {
            this._logger = logger;
            this._dateTimeProvider = dateTimeProvider;
            this._modelConverter = modelConverter;
            this._sessionStore = sessionStore;
        }

        [HttpGet]
        public async Task<ActionResult<SessionSummaries>> Get([FromQuery] DateTime? start, [FromQuery] DateTime? end, [FromQuery] string username)
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vecc.SmtpBitBucket.Core;
using Vecc.SmtpBitBucket.Core.Stores;

namespace Vecc.SmtpBitBucket.Site.Controllers.Api.V1
{
    [ApiController]
    public class Messages : Controller
    {
        private readonly IMessageStore _messageStore;

        public Messages(IMessageStore messageStore)
        {
            this._messageStore = messageStore;
        }

        public async Task<ActionResult<MailMessage>> GetMessages()
        {
            var messageSummaries = await 
        }
    }
}

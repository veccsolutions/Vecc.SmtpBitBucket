using Vecc.SmtpBitBucket.Core;
using Vecc.SmtpBitBucket.Site.Api.V1.Models;

namespace Vecc.SmtpBitBucket.Site.Api.V1.Services.Internal
{
    public class ModelConverter : IModelConverter
    {
        public SessionSummary ToSessionSummary(SmtpSession core)
        {
            if (core == null)
            {
                return null;
            }

            var result = new SessionSummary
            {
                RemoteIp = core.RemoteIp,
                SessionEndTime = core.SessionEndTime,
                SessionId = core.SessionId,
                SessionStartTime = core.SessionStartTime,
                Username = core.Username
            };

            return result;
        }
    }
}

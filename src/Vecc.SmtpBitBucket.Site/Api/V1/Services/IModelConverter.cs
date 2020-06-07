using Vecc.SmtpBitBucket.Core;
using Vecc.SmtpBitBucket.Site.Api.V1.Models;

namespace Vecc.SmtpBitBucket.Site.Api.V1.Services
{
    public interface IModelConverter
    {
        SessionSummary ToSessionSummary(SmtpSession core);
    }
}

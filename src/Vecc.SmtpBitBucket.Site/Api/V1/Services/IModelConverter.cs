using System.Threading.Tasks;
using Vecc.SmtpBitBucket.Core;
using Vecc.SmtpBitBucket.Site.Api.V1.Models;

namespace Vecc.SmtpBitBucket.Site.Api.V1.Services
{
    public interface IModelConverter
    {
        MessageSummaries ToMessageSummaries(Core.MessageSummary[] messages);
        Models.MessageSummary ToMessageSummary(Core.MessageSummary messageSummary);
        SessionSummary ToSessionSummary(SmtpSession core);
        SessionDetails GetSessionDetails(SmtpSession session, SmtpSessionChatter[] sessionChatter, Core.MessageSummary[] messageSummaries);
    }
}

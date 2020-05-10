using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core.Stores
{
    public interface IMessageStore
    {
        Task<MailMessage> GetMessageByIdAsync(int id);
        Task<MessageSummary[]> GetMessageSummariesAsync(string username);
        Task<MailMessage> StoreMessageAsync(MailMessage message);
    }
}

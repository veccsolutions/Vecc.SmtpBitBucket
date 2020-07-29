using System;

namespace Vecc.SmtpBitBucket.Site.Api.V1.Models
{
    public class MessageSummary
    {
        public string MessageId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Subject { get; set; }
        public int Id { get; internal set; }
    }
}

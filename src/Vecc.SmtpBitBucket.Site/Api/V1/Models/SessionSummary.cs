using System;

namespace Vecc.SmtpBitBucket.Site.Api.V1.Models
{
    public class SessionSummary
    {
        public int SessionId { get; set; }
        public DateTime? SessionStartTime { get; set; }
        public DateTime? SessionEndTime { get; set; }
        public string RemoteIp { get; set; }
        public string Username { get; set; }
    }
}

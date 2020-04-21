using System;

namespace Vecc.SmtpBitBucket.Core
{
    public class SmtpSession
    {
        public int SessionId { get; set; }
        public DateTime? SessionStartTime { get; set; }
        public DateTime? SessionEndTime { get; set; }
        public string RemoteIp { get; set; }
        public string Username { get; set; }
    }
}

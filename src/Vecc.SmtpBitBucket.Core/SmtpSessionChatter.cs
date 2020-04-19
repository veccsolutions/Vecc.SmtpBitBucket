using System;

namespace Vecc.SmtpBitBucket.Core
{
    public class SmtpSessionChatter
    {
        public Direction Direction { get; set; }
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }
    }
}

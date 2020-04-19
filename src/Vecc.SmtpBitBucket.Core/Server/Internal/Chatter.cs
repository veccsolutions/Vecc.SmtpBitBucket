using System;

namespace Vecc.SmtpBitBucket.Core.Server.Internal
{
    internal class Chatter
    {
        public Direction Direction { get; set; }
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }
    }
}

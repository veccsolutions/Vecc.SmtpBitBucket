using System;
using Vecc.SmtpBitBucket.Core;

namespace Vecc.SmtpBitBucket.Stores.InMemory
{
    public class Chatter
    {
        public int SessionId { get; set; }
        public SmtpSessionChatter Data { get; set; }
    }
}

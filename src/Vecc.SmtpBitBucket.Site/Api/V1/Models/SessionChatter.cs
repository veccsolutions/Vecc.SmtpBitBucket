using System;

namespace Vecc.SmtpBitBucket.Site.Api.V1.Models
{
    public class SessionChatter
    {
        public ChatterDirection Direction { get; set; }
        public string Data { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}

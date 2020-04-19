using System.Net;

namespace Vecc.SmtpBitBucket.Core.Server
{
    public class ServerOptions
    {
        public bool RequireValidUsernamePassword { get; set; } = false;
        public string ServerHeloResponse { get; set; } = Dns.GetHostName() + " - Vecc.SmtpBitBucket";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core
{
    public class MailMessage
    {
        public string ClientEhlo { get; set; }
        public string Data { get; set; }
        public int Id { get; set; }
        public string MailFrom { get; set; }
        public string[] Receipients { get; set; }
        public string Sender { get; set; }
        public Guid SessionId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Username { get; set; }
        public bool UseUtf8 { get; set; }
    }
}

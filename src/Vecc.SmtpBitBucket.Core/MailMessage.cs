using System;

namespace Vecc.SmtpBitBucket.Core
{
    public class MailMessage
    {
        public string ClientEhlo { get; set; }
        public string Data { get; set; }
        public int Id { get; set; }
        public string MailFrom { get; set; }
        public string[] Receipients { get; set; }
        public int SessionId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Username { get; set; }
        public bool UseUtf8 { get; set; }
        public string Subject { get; internal set; }
    }
}

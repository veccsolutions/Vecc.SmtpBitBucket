using System;

namespace Vecc.SmtpBitBucket.Stores.Postgres.Models
{
    public class DataMailMessage
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string ClientEhlo { get; set; }
        public string MailFrom { get; set; }
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public DateTime ReceiveAt { get; set; }
        public string Username { get; set; }
        public string Data { get; set; }
    }
}

using System;

namespace Vecc.SmtpBitBucket.Stores.Postgres.Models
{
    public class DataChatter
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string Direction { get; set; }
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }
    }
}

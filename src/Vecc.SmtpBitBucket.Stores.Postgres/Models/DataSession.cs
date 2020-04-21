using System;

namespace Vecc.SmtpBitBucket.Stores.Postgres.Models
{
    public class DataSession
    {
        public int Id { get; set; }
        public DateTime SessionStartTime { get; set; }
        public DateTime? SessionEndTime { get; set; }
        public string RemoteIP { get; set; }
        public string Username { get; set; }
    }
}

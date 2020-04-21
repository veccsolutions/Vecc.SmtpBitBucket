namespace Vecc.SmtpBitBucket.Stores.Postgres.Models
{
    public class DataRule
    {
        public int Id { get; set; }
        public string RuleType { get; set; }
        public string RuleConfiguration { get; set; }
    }
}

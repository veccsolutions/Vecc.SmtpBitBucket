
namespace Vecc.SmtpBitBucket.Core.Rules
{
    public class ForwardRule : Rule
    {

        public string DestinationAddress { get; set; }
        public string DestinationName { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }

        public ForwardRule(string data)
            : base(data)
        {
        }

        public override RuleResult Process(MailMessage message, SmtpSession session)
        {
            return RuleResult.None;
        }

    }
}

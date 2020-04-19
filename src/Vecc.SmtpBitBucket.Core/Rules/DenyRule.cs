
namespace Vecc.SmtpBitBucket.Core.Rules
{
    public class DenyRule : Rule
    {
        public DenyRule(string data)
            : base(data)
        {
        }

        public override RuleResult Process(MailMessage message, SmtpSession session)
        {
            return RuleResult.None;
        }
    }
}

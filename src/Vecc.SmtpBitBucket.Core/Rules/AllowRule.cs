
namespace Vecc.SmtpBitBucket.Core.Rules
{
    public class AllowRule : Rule
    {
        public AllowRule(string data)
            : base(data)
        {

        }

        public override RuleResult Process(MailMessage message, SmtpSession session)
        {
            return RuleResult.None;
        }
    }
}

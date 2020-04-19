using System.Text.RegularExpressions;

namespace Vecc.SmtpBitBucket.Core.Rules
{
    public abstract class Rule
    {
        public int Id { get; set; }

        public string FromAddressRegex { get; set; }
        public string FromNameRegex { get; set; }
        public string ToAddressRegex { get; set; }
        public string ToNameRegex { get; set; }
        public string BccAddressRegex { get; set; }
        public string BccNameRegex { get; set; }
        public string CcAddressRegex { get; set; }
        public string CCNameRegex { get; set; }
        public string IpAddressRegex { get; set; }

        public string BodyRegex { get; set; }

        public abstract RuleResult Process(MailMessage message, SmtpSession session);

        public Rule(string data)
        {
        }

        protected bool CheckRule(MimeKit.MimeMessage message, SmtpSession session)
        {
            if (message == null || session == null)
            {
                return false;
            }

            foreach (var address in message.Bcc)
            {
                var mailAddress = address as MimeKit.MailboxAddress;
                if (mailAddress != null)
                {
                }
            }

            return false;
        }

        private bool CheckMailAddress(MimeKit.MailboxAddress address, string addressRegex, string nameRegex)
        {
            return
                address != null &&
                (!string.IsNullOrEmpty(address.Address) && !string.IsNullOrWhiteSpace(addressRegex) && Regex.IsMatch(address.Address, addressRegex)) ||
                (!string.IsNullOrEmpty(address.Name) && !string.IsNullOrWhiteSpace(nameRegex) && Regex.IsMatch(address.Name, nameRegex));
        }
    }
}

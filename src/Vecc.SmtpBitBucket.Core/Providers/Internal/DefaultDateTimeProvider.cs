using System;

namespace Vecc.SmtpBitBucket.Core.Providers.Internal
{
    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset NowOffset => DateTimeOffset.Now;

        public DateTime Now => DateTime.Now;

        public DateTime UtcNow => DateTime.UtcNow;
    }
}

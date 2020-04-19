using System;

namespace Vecc.SmtpBitBucket.Core.Providers
{
    public interface IDateTimeProvider
    {
        DateTimeOffset NowOffset { get; }
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }
}

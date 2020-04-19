using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core.Events
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public Guid SessionId { get; set; }
        public int MessageId { get; set; }

        public DateTime ReceivedAt { get; set; }
    }
}

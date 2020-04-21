using System;
using System.Collections.Generic;

namespace Vecc.SmtpBitBucket.Core.Server
{
    public class SmtpConnection
    {
        public string EhloHost { get; set; }
        public bool UseUtf8 { get; set; }
        public ICollection<string> Recipients { get; set; }
        public ICollection<string> Data { get; set; }
        public int Id { get; set; }
        public string MailFrom { get; set; }
        public string RemoteIp { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
        public string Subject { get; internal set; }

        public SmtpConnection()
        {
            this.Recipients = new List<string>();
            this.Data = new List<string>();
        }

        internal void Reset(bool everything = false)
        {
            this.UseUtf8 = false;
            this.Recipients.Clear();
            this.Data.Clear();

            if (everything)
            {
                this.EhloHost = null;
                this.IsAuthenticated = false;
                this.MailFrom = null;
                this.Username = null;
                this.UseUtf8 = false;
            }
        }
    }
}

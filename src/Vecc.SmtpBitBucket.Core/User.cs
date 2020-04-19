using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vecc.SmtpBitBucket.Core
{
    public class User
    {
        public bool Enabled { get; set; }
        public int Id { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Username { get; set; }
    }
}

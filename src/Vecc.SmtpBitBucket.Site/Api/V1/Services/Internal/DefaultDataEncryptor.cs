using System;
using Microsoft.AspNetCore.DataProtection;

namespace Vecc.SmtpBitBucket.Site.Api.V1.Services.Internal
{
    public class DefaultDataEncryptor : IDataEncryptor
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly Lazy<IDataProtector> _sessionIdDecryptor;
        private readonly Lazy<IDataProtector> _messageIdDecryptor;

        public DefaultDataEncryptor(IDataProtectionProvider dataProtectionProvider)
        {
            this._dataProtectionProvider = dataProtectionProvider;

            this._sessionIdDecryptor = new Lazy<IDataProtector>(() => this._dataProtectionProvider.CreateProtector("V1.SessionId"));
            this._messageIdDecryptor = new Lazy<IDataProtector>(() => this._dataProtectionProvider.CreateProtector("V1.MessageId"));
        }

        public bool TryDecryptMessageId(string data, out int decrypted) => this.TryDecryptInt(data, this._messageIdDecryptor, out decrypted);

        public bool TryDecryptSessionId(string data, out int decrypted) => this.TryDecryptInt(data, this._sessionIdDecryptor, out decrypted);

        public string EncryptMessageId(int data) => this.Encrypt(data, this._messageIdDecryptor);

        public string EncryptSessionId(int data) => this.Encrypt(data, this._sessionIdDecryptor);

        protected bool TryDecryptInt(string data, Lazy<IDataProtector> protector, out int decrypted)
        {
            try
            {
                var protectedData = Convert.FromBase64String(data);
                var unprotected = protector.Value.Unprotect(protectedData);
                var result = BitConverter.ToInt32(unprotected);
                decrypted = result;
                return true;
            }
            catch
            {
                decrypted = default;
                return false;
            }
        }

        protected string Encrypt(int data, Lazy<IDataProtector> protector)
        {
            var unprotected = BitConverter.GetBytes(data);
            var protectedData = protector.Value.Protect(unprotected);
            var result = Convert.ToBase64String(protectedData);

            return result;
        }
    }
}

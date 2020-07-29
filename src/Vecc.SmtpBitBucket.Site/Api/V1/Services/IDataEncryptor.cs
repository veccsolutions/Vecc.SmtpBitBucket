namespace Vecc.SmtpBitBucket.Site.Api.V1.Services
{
    public interface IDataEncryptor
    {
        string EncryptMessageId(int id);
        string EncryptSessionId(int data);

        bool TryDecryptMessageId(string data, out int decrypted);
        bool TryDecryptSessionId(string data, out int decrypted);
    }
}

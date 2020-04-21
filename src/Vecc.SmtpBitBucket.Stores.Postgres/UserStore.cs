using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Vecc.SmtpBitBucket.Core.Stores;

namespace Vecc.SmtpBitBucket.Stores.Postgres
{
    public class UserStore : Database, IUserStore
    {
        public UserStore(IOptions<DatabaseOptions> databaseOptions) : base(databaseOptions)
        {
        }

        public async Task CreateUserAsync(string username, string password)
        {
            //We'll use sha512 for the hash
            var salt = new byte[64]; // 512 / 8

            using (var rng = RandomNumberGenerator.Create())
            using (var sha512 = SHA512.Create())
            using (var connection = this.GetConnection())
            {
                rng.GetBytes(salt);
                var passwordBytes = Encoding.ASCII.GetBytes(password);
                var saltAndPassword = salt.Union(passwordBytes).ToArray();
                var hash = sha512.ComputeHash(saltAndPassword);
                var saltAndHash = salt.Union(hash).ToArray();
                var encoded = Convert.ToBase64String(saltAndHash);

                await connection.ExecuteAsync(@"
INSERT INTO ""SmtpUsers""
(""Username"", ""Password"", ""Enabled"")
VALUES
(@username, @password, 1)
",
                    new
                    {
                        username = username,
                        password = encoded
                    });
            }

        }

        public async Task DisableUserAsync(string username)
        {
            using (var connection = this.GetConnection())
            {
                await connection.ExecuteAsync("UPDATE \"SmtpUsers\" SET \"Enabled\" = 0 WHERE \"Username\" = @username", new { username = username });
            }
        }

        public async Task<bool> IsUserEnabledAsync(string username)
        {
            using (var connection = this.GetConnection())
            {
                var result = await connection.ExecuteScalarAsync<bool>("SELECT \"Enabled\" FROM \"SmtpUsers\" WHERE \"Username\" = @username", new { username = username });
                return result;
            }
        }

        public async Task<bool> IsValidAsync(string username, string password)
        {
            using (var sha512 = SHA512.Create())
            using (var connection = this.GetConnection())
            {
                var hashedPassword = await connection.ExecuteScalarAsync<string>("SELECT \"Password\" FROM \"SmtpUsers\" WHERE \"Username\" = @username", new { username = username });
                if (string.IsNullOrWhiteSpace(hashedPassword))
                {
                    return false;
                }

                var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);
                var passwordBytes = Encoding.ASCII.GetBytes(password);
                var saltAndPassword = hashedPasswordBytes.Take(64).Union(passwordBytes).ToArray();
                var hash = sha512.ComputeHash(saltAndPassword);
                var base64Hash = Convert.ToBase64String(hash);
                var base64Password = Convert.ToBase64String(hashedPasswordBytes.Skip(64).ToArray());

                var result = base64Hash == base64Password;
                return result;
            }
        }
    }
}

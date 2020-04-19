using System.Collections.Generic;
using System.Threading.Tasks;
using Vecc.SmtpBitBucket.Core.Stores;

namespace Vecc.SmtpBitBucket.Stores.InMemory
{
    public class UserStore : IUserStore
    {
        private readonly Dictionary<string, string> _userStore = new Dictionary<string, string>();

        public Task CreateUserAsync(string username, string password)
        {
            this._userStore[username] = password;
            return Task.CompletedTask;
        }

        public Task DisableUserAsync(string username)
        {
            this._userStore[username] = string.Empty;
            return Task.CompletedTask;
        }

        public Task<bool> IsUserEnabledAsync(string username)
        {
            var result = false;

            if (this._userStore.TryGetValue(username, out var password))
            {
                result = !string.IsNullOrWhiteSpace(password);
            }

            return Task.FromResult(result);
        }

        public async Task<bool> IsValidAsync(string username, string password)
        {
            var result = false;

            if (await this.IsUserEnabledAsync(username))
            {
                result = this._userStore[username] == password;
            }

            return result;
        }
    }
}

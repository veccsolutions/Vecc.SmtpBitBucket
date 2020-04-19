using System.Threading.Tasks;
using Vecc.SmtpBitBucket.Core.Rules;
using Vecc.SmtpBitBucket.Core.Stores;

namespace Vecc.SmtpBitBucket.Stores.InMemory
{
    public class RuleStore : IRuleStore
    {
        private readonly Rule[] _rules = new Rule[0];

        public Task<Rule[]> GetRulesAsync() => Task.FromResult(this._rules);
    }
}

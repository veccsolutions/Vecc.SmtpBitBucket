using System.Threading.Tasks;
using Vecc.SmtpBitBucket.Core.Rules;

namespace Vecc.SmtpBitBucket.Core.Stores
{
    public interface IRuleStore
    {
        Task<Rule[]> GetRulesAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Vecc.SmtpBitBucket.Core.Rules;
using Vecc.SmtpBitBucket.Core.Stores;
using Vecc.SmtpBitBucket.Stores.Postgres.Models;

namespace Vecc.SmtpBitBucket.Stores.Postgres
{
    public class RuleStore : Database, IRuleStore
    {
        public RuleStore(IOptions<DatabaseOptions> databaseOptions)
            : base(databaseOptions)
        {
        }

        public async Task<Rule[]> GetRulesAsync()
        {
            using (var connection = this.GetConnection())
            {
                var dataRules = await connection.QueryAsync<DataRule>("SELECT * FROM \"Rules\"");
                var result = new List<Rule>();

                foreach (var dataRule in dataRules)
                {
                    var ruleType = Type.GetType(dataRule.RuleType, false);
                    if (ruleType != null)
                    {
                        var ruleObject = JsonSerializer.Deserialize(dataRule.RuleConfiguration, ruleType);
                        if (ruleObject is Rule rule)
                        {
                            result.Add(rule);
                        }
                    }
                }

                return result.ToArray();
            }
        }
    }
}

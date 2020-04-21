using Microsoft.Extensions.Options;

namespace Vecc.SmtpBitBucket.Stores.Postgres
{
    public class Database
    {
        private readonly DatabaseOptions _databaseOptions;

        public Database(IOptions<DatabaseOptions> databaseOptions)
        {
            this._databaseOptions = databaseOptions.Value;
        }

        public Npgsql.NpgsqlConnection GetConnection() => new Npgsql.NpgsqlConnection(this._databaseOptions.ConnectionString);
    }
}

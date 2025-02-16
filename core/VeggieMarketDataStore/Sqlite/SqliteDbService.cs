using System.Data.Common;
using System.Data.SQLite;
using VeggieMarketDataStore.DbInterfaces;
using VeggieMarketLogger;

namespace VeggieMarketDataStore
{
    public class SqliteDbService : DbService
    {
        private const string CONNECTION_STRING = @"Data Source=C:\Stuff\Projects\veggie\db\veggie.sqlite";

        public SqliteDbService(ILogger logger) : base(logger)
        {
            
        }

        public override DbConnection OpenConnection()
        {
            if (connection == null)
            {
                connection = new SQLiteConnection(CONNECTION_STRING);
                connection.Open();
            }
            return connection;
        }

        public override DbCommand CreateCommand(string query, DbConnection connection)
        {
            return new SQLiteCommand(query, connection as SQLiteConnection);
        }
    }
}

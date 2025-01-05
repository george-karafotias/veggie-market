using System.Data.Common;
using System.Data.SQLite;
using VeggieMarketDataStore.DbInterfaces;

namespace VeggieMarketDataStore
{
    public class SqliteDbService : DbService
    {
        private const string CONNECTION_STRING = @"Data Source=C:\Stuff\Projects\veggie\db\veggie.sqlite";

        public override DbConnection OpenConnection()
        {
            SQLiteConnection connection = new SQLiteConnection(CONNECTION_STRING);
            connection.Open();
            return connection;
        }

        public override DbCommand CreateCommand(string query, DbConnection connection)
        {
            return new SQLiteCommand(query, connection as SQLiteConnection);
        }
    }
}

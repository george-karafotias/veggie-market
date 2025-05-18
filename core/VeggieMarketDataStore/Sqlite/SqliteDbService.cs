using System.Data.Common;
using System.Data.SQLite;
using VeggieMarketDataStore.DbInterfaces;
using VeggieMarketLogger;

namespace VeggieMarketDataStore
{
    public class SqliteDbService : DbService
    {
        private string connectionString = @"Data Source=C:\Stuff\Projects\veggie\db\veggie.sqlite";
        //private string connectionString = "";

        public SqliteDbService(ILogger logger) : base(logger)
        {
            
        }

        public SqliteDbService(string path, ILogger logger) : base(logger)
        {
            this.connectionString = @"Data Source=" + @path;
        }

        public override DbConnection OpenConnection()
        {
            if (connection == null)
            {
                connection = new SQLiteConnection(connectionString);
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

using System.Collections.Generic;
using System.Data.Common;
using VeggieMarketDataStore.DbInterfaces;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore
{
    public class MarketDbService : IMarketDbService
    {
        private readonly DbService dbService;

        public MarketDbService(DbService dbService)
        {
            this.dbService = dbService;
        }

        public bool InsertMarket(Market market)
        {
            string query = "INSERT INTO Markets(MarketName) VALUES(@marketName)";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@marketName", market.MarketName }
            };
            return dbService.Insert(query, parameters);
        }

        public Market GetMarket(string marketName)
        {
            Market market = null;

            string query = "SELECT * FROM Markets WHERE MarketName = @marketName";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@marketName", marketName }
            };
            DbConnection connection = dbService.OpenConnection();
            DbDataReader reader = dbService.Select(connection, query, parameters);
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    market = dbService.CreateMarket(reader);
                    break;
                }
            }

            dbService.CloseConnection(connection, reader);
            return market;
        }
    }
}

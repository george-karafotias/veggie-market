using VeggieMarketDataStore;
using VeggieMarketLogger;

namespace VeggieMarketApi
{
    public static class DbServiceProvider
    {
        public static DataStorageService GetDataStorageService(ILogger logger = null)
        {
            if (logger == null)
            {
                logger = Logger.GetInstance();
            }

            return DataStorageService.GetInstance(new SqliteDbService(logger), logger);
        }
    }
}

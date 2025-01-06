using VeggieMarketDataStore;

namespace VeggieMarketApi
{
    public static class DbServiceProvider
    {
        public static DataStorageService GetDataStorageService()
        {
            return DataStorageService.GetInstance(new SqliteDbService());
        }
    }
}

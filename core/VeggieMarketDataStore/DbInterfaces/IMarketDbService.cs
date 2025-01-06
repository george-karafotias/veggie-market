using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore.DbInterfaces
{
    public interface IMarketDbService
    {
        bool InsertMarket(Market market);

        Market[] GetMarkets();

        Market GetMarket(string marketName);
    }
}

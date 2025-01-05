using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore
{
    public interface IMarketDbService
    {
        bool InsertMarket(Market market);

        Market GetMarket(string marketName);
    }
}

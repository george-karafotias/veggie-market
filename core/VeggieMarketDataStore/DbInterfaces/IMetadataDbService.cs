using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore.DbInterfaces
{
    public interface IMetadataDbService
    {
        MarketAvailableData[] GetAvailablePrices();

        DatePeriod[] GetMarketAvailablePrices(Market market);
    }
}

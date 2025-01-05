using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore.DbInterfaces
{
    public interface IMetadataDbService
    {
        DatePeriod[] GetAvailableDatePeriods();
    }
}

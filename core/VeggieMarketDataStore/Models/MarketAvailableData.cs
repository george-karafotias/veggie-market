using System.Collections.Generic;

namespace VeggieMarketDataStore.Models
{
    public class MarketAvailableData
    {
        private Market market;
        private DatePeriod[] datePeriods;

        public MarketAvailableData(Market market, DatePeriod[] datePeriods)
        {
            this.market = market;
            this.datePeriods = datePeriods;
        }

        public Market Market { get { return market; } }
        public DatePeriod[] DatePeriods { get { return datePeriods; } }
    }
}

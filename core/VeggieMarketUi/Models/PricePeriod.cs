using VeggieMarketDataStore.Models;

namespace VeggieMarketUi.Models
{
    public class PricePeriod
    {
        private string marketName;
        private string startDate;
        private string endDate;

        public PricePeriod(Market market, DatePeriod datePeriod) 
        {
            marketName = market.MarketName;
            startDate = datePeriod.StartDate;
            endDate = datePeriod.EndDate;
        }

        public string MarketName { get { return marketName; } }
        public string StartDate { get { return startDate; } }
        public string EndDate { get { return endDate; } }
    }
}

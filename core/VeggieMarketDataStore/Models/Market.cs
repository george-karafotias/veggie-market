namespace VeggieMarketDataStore.Models
{
    public class Market
    {
        public int MarketId { get; set; }

        public string MarketName { get; set; }

        public Market(string marketName)
        {
            MarketName = marketName;
        }

        public Market(int marketId, string marketName) : this(marketName)
        {
            MarketId = marketId;
        }
    }
}

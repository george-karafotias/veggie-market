
using System;

namespace VeggieMarketUi.Models
{
    public class PriceRetrievalParameters
    {
        public int ProductId { get; set; }
        public int MarketId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}


using System;
using System.Collections.Generic;
using VeggieMarketDataStore.Models;

namespace VeggieMarketUi.Models
{
    public class PriceRetrievalParameters
    {
        public List<Product> Products { get; set; }
        public List<Market> Markets { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

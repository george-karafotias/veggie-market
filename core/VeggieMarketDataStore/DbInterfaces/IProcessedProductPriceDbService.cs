using System;
using System.Collections.Generic;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore.DbInterfaces
{
    public interface IProcessedProductPriceDbService
    {
        bool InsertProcessedPrice(ProductPrice productPrice);

        IEnumerable<ProductPrice> GetProcessedPrices(DateTime? fromDate, DateTime? toDate);

        IEnumerable<ProductPrice> GetProcessedProductPrices(int productId, DateTime? fromDate, DateTime? toDate);

        IEnumerable<ProductPrice> GetProcessedProductMarketPrices(int productId, int marketId, DateTime? fromDate, DateTime? toDate);
    }
}

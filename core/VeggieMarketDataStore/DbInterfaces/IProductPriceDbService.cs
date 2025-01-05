using System;
using System.Collections.Generic;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore
{
    public interface IProductPriceDbService
    {
        bool InsertPrice(ProductPrice productPrice);

        IEnumerable<ProductPrice> GetPrices(DateTime? fromDate, DateTime? toDate);

        IEnumerable<ProductPrice> GetProductPrices(int productId, DateTime? fromDate, DateTime? toDate);

        IEnumerable<ProductPrice> GetProductMarketPrices(int productId, int marketId, int year);

        bool ProductHasPrice(int productId, long date);
    }
}

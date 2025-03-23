using System;
using System.Collections.Generic;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore.DbInterfaces
{
    public interface IProductPriceDbService
    {
        bool InsertPrice(ProductPrice productPrice);

        IEnumerable<ProductPrice> GetPrices(DateTime? fromDate, DateTime? toDate);

        IEnumerable<ProductPrice> GetProductPrices(int productId, DateTime? fromDate, DateTime? toDate);

        IEnumerable<ProductPrice> GetProductMarketPrices(int productId, int marketId, DateTime? fromDate, DateTime? toDate);

        bool ProductHasPrice(int productId, int marketId, long date);
    }
}

using System;
using System.Collections.Generic;
using VeggieMarketDataStore.DbInterfaces;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore.DbServices
{
    public class ProcessedProductPriceDbService : IProcessedProductPriceDbService
    {
        private const string PRODUCT_PRICES_TABLE = "ProcessedProductPrices";
        private readonly DbService dbService;
        private readonly ProductPriceDbProcessor productPriceDbProcessor;

        public ProcessedProductPriceDbService(DbService dbService)
        {
            this.dbService = dbService;
            productPriceDbProcessor = new ProductPriceDbProcessor(this.dbService);
        }

        public bool InsertProcessedPrice(ProductPrice productPrice)
        {
            return productPriceDbProcessor.InsertProductPrice(PRODUCT_PRICES_TABLE, productPrice);
        }

        public IEnumerable<ProductPrice> GetProcessedPrices(DateTime? fromDate, DateTime? toDate)
        {
            return productPriceDbProcessor.GetPrices(PRODUCT_PRICES_TABLE, fromDate, toDate);
        }

        public IEnumerable<ProductPrice> GetProcessedProductPrices(int productId, DateTime? fromDate, DateTime? toDate)
        {
            return productPriceDbProcessor.GetProductPrices(PRODUCT_PRICES_TABLE, productId, fromDate, toDate);
        }

        public IEnumerable<ProductPrice> GetProcessedProductMarketPrices(int productId, int marketId, int year)
        {
            return productPriceDbProcessor.GetProductMarketPrices(PRODUCT_PRICES_TABLE, productId, marketId, year);
        }
    }
}

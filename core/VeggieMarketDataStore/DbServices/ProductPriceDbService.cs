﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using VeggieMarketDataStore.DbInterfaces;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore.DbServices
{
    public class ProductPriceDbService : IProductPriceDbService
    {
        private const string PRODUCT_PRICES_TABLE = "ProductPrices";
        private readonly DbService dbService;
        private readonly ProductPriceDbProcessor productPriceDbProcessor;

        public ProductPriceDbService(DbService dbService)
        {
            this.dbService = dbService;
            productPriceDbProcessor = new ProductPriceDbProcessor(this.dbService);
        }

        public bool InsertPrice(ProductPrice productPrice)
        {
            return productPriceDbProcessor.InsertProductPrice(PRODUCT_PRICES_TABLE, productPrice);
        }

        public IEnumerable<ProductPrice> GetPrices(DateTime? fromDate, DateTime? toDate)
        {
            return productPriceDbProcessor.GetPrices(PRODUCT_PRICES_TABLE, fromDate, toDate);
        }

        public IEnumerable<ProductPrice> GetProductPrices(int productId, DateTime? fromDate, DateTime? toDate)
        {
            return productPriceDbProcessor.GetProductPrices(PRODUCT_PRICES_TABLE, productId, fromDate, toDate);
        }

        public IEnumerable<ProductPrice> GetProductMarketPrices(int productId, int marketId, DateTime? fromDate, DateTime? toDate)
        {
            return productPriceDbProcessor.GetProductMarketPrices(PRODUCT_PRICES_TABLE, productId, marketId, fromDate, toDate);
        }

        public bool ProductHasPrice(int productId, int marketId, long date)
        {
            bool retVal = false;

            string query = "SELECT * FROM ProductPrices WHERE Product = @productId AND Market = @marketId AND Date = @date";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@productId", productId },
                { "@marketId", marketId },
                { "@date", date }
            };
            DbConnection connection = dbService.OpenConnection();
            DbDataReader reader = dbService.Select(connection, query, parameters);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    retVal = true;
                    break;
                }
            }

            dbService.CloseConnection(connection, reader);
            return retVal;
        }
    }
}

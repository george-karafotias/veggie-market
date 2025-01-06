using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using VeggieMarketDataStore.DbInterfaces;
using VeggieMarketDataStore.Models;
using VeggieMarketLogger;

namespace VeggieMarketDataStore
{
    public class ProductPriceDbProcessor
    {
        private const string PRODUCT_PRICE_DATE_COLUMN_NAME = "Date";
        private readonly DbService dbService;
        private readonly Logger logger;

        public ProductPriceDbProcessor(DbService dbService)
        {
            this.dbService = dbService;
            logger = Logger.GetInstance();
        }

        public IEnumerable<ProductPrice> GetPrices(string pricesTable, DateTime? fromDate, DateTime? toDate)
        {
            return GetProductPricesInternal(pricesTable, GetProductDateFilter(fromDate, toDate));
        }

        public IEnumerable<ProductPrice> GetProductPrices(string pricesTable, int productId, DateTime? fromDate, DateTime? toDate)
        {
            string filter = CreateProductPricesFilter(productId, fromDate, toDate);
            return GetProductPricesInternal(pricesTable, filter);
        }

        public IEnumerable<ProductPrice> GetProductMarketPrices(string productPricesTable, int productId, int marketId, int year)
        {
            string filter = CreateProductPricesFilter(productId, GetYearStartDate(year), GetYearEndDate(year), marketId);
            return GetProductPricesInternal(productPricesTable, filter);
        }

        public bool InsertProductPrice(string pricesTable, ProductPrice productPrice)
        {
            if (ProductPriceExists(productPrice, pricesTable))
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "productPrice for product " + productPrice.Product.ProductId + " already exists for date " + productPrice.ProductDate + " in market " + productPrice.Market.MarketId, Logger.LogType.Warning);
                return false;
            }

            string query = "INSERT INTO " + pricesTable + "(Product, Date, Market, ExtraCategory, Category1MinPrice, Category1MaxPrice, " +
                    "Category2MinPrice, Category2MaxPrice, QuantityToSupply, DominantPrice, PreviousWeekDominantPrice, " +
                    "PreviousYearDominantPrice, PreviousWeekPriceDifference, PreviousYearPriceDifference, SoldQuantityPercentage" +
                    ") VALUES(@productId, @productDate, @marketId, @extraCategory, @category1MinPrice, @category1MaxPrice, " +
                    "@category2MinPrice, @category2MaxPrice, @quantityToSupply, @dominantPrice, @previousWeekDominantPrice, " +
                    "@previousYearDominantPrice, @previousWeekPriceDifference, @previousYearPriceDifference, @soldQuantityPercentage)";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@productId", productPrice.Product.ProductId },
                { "@productDate", productPrice.ProductDate },
                { "@marketId", productPrice.Market.MarketId },
                { "@extraCategory", productPrice.ExtraCategory },
                { "@category1MinPrice", productPrice.Category1MinPrice },
                { "@category1MaxPrice", productPrice.Category1MaxPrice },
                { "@category2MinPrice", productPrice.Category2MinPrice },
                { "@category2MaxPrice", productPrice.Category2MaxPrice },
                { "@quantityToSupply", productPrice.QuantityToSupply },
                { "@dominantPrice", productPrice.DominantPrice },
                { "@previousWeekDominantPrice", productPrice.PreviousWeekDominantPrice },
                { "@previousYearDominantPrice", productPrice.PreviousYearDominantPrice },
                { "@previousWeekPriceDifference", productPrice.PreviousWeekPriceDifference },
                { "@previousYearPriceDifference", productPrice.PreviousYearPriceDifference },
                { "@soldQuantityPercentage", productPrice.SoldQuantityPercentage }
            };

            return dbService.Insert(query, parameters);
        }

        private IEnumerable<ProductPrice> GetProductPricesInternal(string pricesTable, string filter)
        {
            List<ProductPrice> productPrices = new List<ProductPrice>();

            string query = "SELECT * FROM " + pricesTable + " INNER JOIN Markets ON " + pricesTable + ".Market = Markets.MarketId INNER JOIN Products ON " + pricesTable + ".Product = Products.ProductId INNER JOIN ProductTypes ON Products.ProductType = ProductTypes.ProductTypeId";
            if (!string.IsNullOrEmpty(filter))
            {
                query += " WHERE " + filter;
            }
            query += " ORDER BY Date;";
            DbConnection connection = dbService.OpenConnection();
            DbDataReader reader = dbService.Select(connection, query);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ProductPrice productPrice = CreateProductPrice(reader);
                    productPrices.Add(productPrice);
                }
            }

            dbService.CloseConnection(connection, reader);
            return productPrices;
        }

        private ProductPrice CreateProductPrice(DbDataReader reader)
        {
            Product product = dbService.CreateProduct(reader);
            Market market = dbService.CreateMarket(reader);
            long productDate = Convert.ToInt64(reader.GetValue(reader.GetOrdinal(PRODUCT_PRICE_DATE_COLUMN_NAME)));
            ProductPrice productPrice = new ProductPrice(product, productDate, market);

            productPrice.ExtraCategory = dbService.ConvertDbValueToNullableDouble(reader.GetValue(reader.GetOrdinal("ExtraCategory")));
            productPrice.Category1MinPrice = dbService.ConvertDbValueToNullableDouble(reader.GetValue(reader.GetOrdinal("Category1MinPrice")));
            productPrice.Category1MaxPrice = dbService.ConvertDbValueToNullableDouble(reader.GetValue(reader.GetOrdinal("Category1MaxPrice")));
            productPrice.Category2MinPrice = dbService.ConvertDbValueToNullableDouble(reader.GetValue(reader.GetOrdinal("Category2MinPrice")));
            productPrice.Category2MaxPrice = dbService.ConvertDbValueToNullableDouble(reader.GetValue(reader.GetOrdinal("Category2MaxPrice")));
            productPrice.QuantityToSupply = dbService.ConvertDbValueToNullableDouble(reader.GetValue(reader.GetOrdinal("QuantityToSupply")));
            productPrice.DominantPrice = dbService.ConvertDbValueToNullableDouble(reader.GetValue(reader.GetOrdinal("DominantPrice")));
            productPrice.PreviousWeekDominantPrice = dbService.ConvertDbValueToNullableDouble(reader.GetValue(reader.GetOrdinal("PreviousWeekDominantPrice")));
            productPrice.PreviousYearDominantPrice = dbService.ConvertDbValueToNullableDouble(reader.GetValue(reader.GetOrdinal("PreviousYearDominantPrice")));
            productPrice.PreviousWeekPriceDifference = dbService.ConvertDbValueToNullableDouble(reader.GetValue(reader.GetOrdinal("PreviousWeekPriceDifference")));
            productPrice.PreviousYearPriceDifference = dbService.ConvertDbValueToNullableDouble(reader.GetValue(reader.GetOrdinal("PreviousYearPriceDifference")));
            productPrice.SoldQuantityPercentage = dbService.ConvertDbValueToNullableDouble(reader.GetValue(reader.GetOrdinal("SoldQuantityPercentage")));

            return productPrice;
        }

        private bool ProductPriceExists(ProductPrice productPrice, string productPricesTable)
        {
            IEnumerable<ProductPrice> productPrices = GetProductPricesInternal(productPricesTable, PRODUCT_PRICE_DATE_COLUMN_NAME + " = " + productPrice.ProductDate + " AND Market = " + productPrice.Market.MarketId + " AND Product = " + productPrice.Product.ProductId);
            return productPrices.ToList().Count > 0;
        }

        private string GetProductDateFilter(DateTime? fromDate, DateTime? toDate)
        {
            string retVal = "";

            if (fromDate.HasValue)
            {
                retVal += PRODUCT_PRICE_DATE_COLUMN_NAME + ">=" + fromDate.Value.Ticks;
            }
            if (toDate.HasValue)
            {
                if (!string.IsNullOrEmpty(retVal))
                {
                    retVal += " AND ";
                }
                retVal += PRODUCT_PRICE_DATE_COLUMN_NAME + "<=" + toDate.Value.Ticks;
            }

            return retVal;
        }

        private string CreateProductPricesFilter(int productId, DateTime? fromDate, DateTime? toDate, int marketId = -1)
        {
            List<string> filters = new List<string>
            {
                GetProductIdFilter(productId),
                GetProductDateFilter(fromDate, toDate)
            };
            if (marketId != -1)
            {
                filters.Add(GetMarketIdFilter(marketId));
            }
            return dbService.MergeFilters(filters);
        }

        private string GetProductIdFilter(int productId)
        {
            return "Product = " + productId;
        }

        private string GetMarketIdFilter(int marketId)
        {
            return "Market = " + marketId;
        }

        private DateTime GetYearStartDate(int year)
        {
            return new DateTime(year, 1, 1);
        }

        private DateTime GetYearEndDate(int year)
        {
            return new DateTime(year, 12, 31);
        }
    }
}

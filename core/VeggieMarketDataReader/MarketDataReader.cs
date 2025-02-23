using System;
using System.Collections.Generic;
using System.Reflection;
using VeggieMarketDataStore.Models;
using VeggieMarketLogger;

namespace VeggieMarketDataReader
{
    public abstract class MarketDataReader
    {
        public abstract DateTime? ReadSingleDay(string file);

        protected VeggieMarketDataStore.DataStorageService dataStorageService;
        protected ILogger logger;
        protected Market market;

        public MarketDataReader(VeggieMarketDataStore.DataStorageService dataStorageService)
        {
            this.dataStorageService = dataStorageService;
            logger = this.dataStorageService.Logger;
        }

        public ILogger Logger
        {
            set
            {
                this.dataStorageService.Logger = value;
                this.logger = value;
            }
        }

        public DateTime[] ReadMultipleDays(string[] files)
        {
            try
            {
                if (files == null) return null;

                List<DateTime> days = new List<DateTime>();
                foreach (string file in files)
                {
                    logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Processing " + file + "...", LogType.Info);
                    DateTime? day = ReadSingleDay(file);
                    if (day.HasValue)
                    {
                        days.Add(day.Value);
                    }
                    else
                    {
                        logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Failed to process " + file + "...", LogType.Warning);
                    }
                }

                return days.ToArray();
            }
            catch (Exception ex)
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, ex.StackTrace, LogType.Exception);
                return null;
            }
        }

        protected void GetMarket(string marketName)
        {
            market = dataStorageService.MarketDbService.GetMarket(marketName);
            if (market == null)
            {
                market = new Market(marketName);
                bool inserted = dataStorageService.MarketDbService.InsertMarket(market);
                market = inserted ? dataStorageService.MarketDbService.GetMarket(marketName) : null;
            }
        }

        protected ProductType ParseProductType(string productTypeName)
        {
            ProductType productType = dataStorageService.ProductTypeDbService.GetProductType(productTypeName, true);
            if (productType == null)
            {
                productType = new ProductType(productTypeName);
                bool productTypeInserted = dataStorageService.ProductTypeDbService.InsertProductType(productType);
                productType = productTypeInserted ? dataStorageService.ProductTypeDbService.GetProductType(productTypeName) : null;
            }

            return productType;
        }

        protected double[] ExtractMinMaxPrice(string price)
        {
            double[] minMaxPrice = null;
            if (!string.IsNullOrEmpty(price))
            {
                string[] priceParts = price.Split(new char[] { '-' });
                if (priceParts != null && priceParts.Length == 2)
                {
                    priceParts[0] = NormalizePrice(priceParts[0].Trim());
                    priceParts[1] = NormalizePrice(priceParts[1].Trim());
                    if (double.TryParse(priceParts[0], out double minPrice) && double.TryParse(priceParts[1], out double maxPrice))
                    {
                        minMaxPrice = new double[priceParts.Length];
                        minMaxPrice[0] = minPrice;
                        minMaxPrice[1] = maxPrice;
                    }
                }
            }
            return minMaxPrice;
        }

        protected string NormalizePrice(string price)
        {
            string normalizedPrice = "";
            if (!string.IsNullOrEmpty(price))
            {
                normalizedPrice = price.Replace(',', '.').Trim();
            }
            return normalizedPrice;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VeggieMarketDataStore;
using VeggieMarketDataStore.Models;
using VeggieMarketLogger;

namespace VeggieMarketDataProcessor
{
    public class DataProcessor
    {
        private DataStorageService dataStorageService;
        private ILogger logger;

        public DataProcessor(DataStorageService dataStorageService) 
        {
            this.dataStorageService = dataStorageService;
            this.logger = dataStorageService.Logger;
        }

        public void ProcessProductPrices(string marketName, DateTime[] days)
        {
            logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Processing product prices for market " + marketName, LogType.Info);

            Market market = dataStorageService.MarketDbService.GetMarket(marketName);
            IEnumerable<Product> products = dataStorageService.ProductDbService.GetProducts();
            if (market == null || products == null)
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Aborting processing as no market or no products were found", LogType.Error);
                return;
            }

            Dictionary<int, List<DateTime>> daysByYear = SplitDaysByYear(days);
            if (daysByYear.Count() == 0)
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Aborting processing as no dates were found", LogType.Error);
                return;
            }

            foreach (KeyValuePair<int, List<DateTime>> daysByYearPair in daysByYear)
            {
                int year = daysByYearPair.Key;
                DateTime fromDate = new DateTime(year, 1, 1);
                DateTime toDate = new DateTime(year, 12, 31);

                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Processing year " + year, LogType.Info);

                foreach (Product product in products)
                {
                    logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Processing product " + product.ProductName, LogType.Info);

                    //are there any prices to process? if no, go to the next product
                    IEnumerable<ProductPrice> productPrices = dataStorageService.ProductPriceDbService.GetProductMarketPrices(product.ProductId, market.MarketId, fromDate, toDate);
                    if (productPrices == null || productPrices.Count() == 0) continue;

                    //maybe the prices have already been processed before? if yes, go to the next product
                    IEnumerable<ProductPrice> existingProcessedProductPrices = dataStorageService.ProcessedProductPriceDbService.GetProcessedProductMarketPrices(product.ProductId, market.MarketId, fromDate, toDate);
                    if (existingProcessedProductPrices != null && existingProcessedProductPrices.Count() > 0) continue;

                    IEnumerable<ProductPrice> processedProductPrices = ProcessProductPrices(productPrices, fromDate, toDate);
                    if (processedProductPrices == null || processedProductPrices.Count() == 0) continue;

                    foreach (ProductPrice productPrice in processedProductPrices)
                    {
                        dataStorageService.ProcessedProductPriceDbService.InsertProcessedPrice(productPrice);
                    }
                }
            }
        }

        private IEnumerable<ProductPrice> ProcessProductPrices(IEnumerable<ProductPrice> productPrices, DateTime begin, DateTime end)
        {
            List<ProductPrice> processedProductPrices = new List<ProductPrice>();
            Product product = productPrices.First().Product;
            Market market = productPrices.First().Market;

            for (DateTime date = begin; date <= end; date = date.AddDays(1))
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Processing date " + date.ToString(), LogType.Info);

                ProductPrice productPrice = ProductPriceExists(productPrices, date);
                if (productPrice == null)
                {
                    processedProductPrices.Add(new ProductPrice(product, date.Ticks, market));
                }
                else
                {
                    processedProductPrices.Add(productPrice);
                }
            }

            double?[] extraCategory = new double?[processedProductPrices.Count];
            double?[] category1MinPrice = new double?[processedProductPrices.Count];
            double?[] category1MaxPrice = new double?[processedProductPrices.Count];
            double?[] category2MinPrice = new double?[processedProductPrices.Count];
            double?[] category2MaxPrice = new double?[processedProductPrices.Count];
            double?[] quantityToSupply = new double?[processedProductPrices.Count];
            double?[] dominantPrice = new double?[processedProductPrices.Count];
            double?[] previousWeekDominantPrice = new double?[processedProductPrices.Count];
            double?[] previousYearDominantPrice = new double?[processedProductPrices.Count];
            double?[] previousWeekPriceDifference = new double?[processedProductPrices.Count];
            double?[] previousYearPriceDifference = new double?[processedProductPrices.Count];
            double?[] soldQuantityPercentage = new double?[processedProductPrices.Count];
            for (int i = 0; i < processedProductPrices.Count; i++)
            {
                ProductPrice processedProductPrice = processedProductPrices[i];
                extraCategory[i] = processedProductPrice.ExtraCategory;
                category1MinPrice[i] = processedProductPrice.Category1MinPrice;
                category1MaxPrice[i] = processedProductPrice.Category1MaxPrice;
                category2MinPrice[i] = processedProductPrice.Category2MinPrice;
                category2MaxPrice[i] = processedProductPrice.Category2MaxPrice;
                quantityToSupply[i] = processedProductPrice.QuantityToSupply;
                dominantPrice[i] = processedProductPrice.DominantPrice;
                previousWeekDominantPrice[i] = processedProductPrice.PreviousWeekDominantPrice;
                previousYearDominantPrice[i] = processedProductPrice.PreviousYearDominantPrice;
                previousWeekPriceDifference[i] = processedProductPrice.PreviousWeekPriceDifference;
                previousYearPriceDifference[i] = processedProductPrice.PreviousYearPriceDifference;
                soldQuantityPercentage[i] = processedProductPrice.SoldQuantityPercentage;
            }

            Interpolation.LinSpaceArray(ref extraCategory);
            Interpolation.LinSpaceArray(ref category1MinPrice);
            Interpolation.LinSpaceArray(ref category1MaxPrice);
            Interpolation.LinSpaceArray(ref category2MinPrice);
            Interpolation.LinSpaceArray(ref category2MaxPrice);
            Interpolation.LinSpaceArray(ref quantityToSupply);
            Interpolation.LinSpaceArray(ref dominantPrice);
            Interpolation.LinSpaceArray(ref previousWeekDominantPrice);
            Interpolation.LinSpaceArray(ref previousYearDominantPrice);
            Interpolation.LinSpaceArray(ref previousWeekPriceDifference);
            Interpolation.LinSpaceArray(ref previousYearPriceDifference);
            Interpolation.LinSpaceArray(ref soldQuantityPercentage);

            for (int i = 0; i < processedProductPrices.Count; i++)
            {
                processedProductPrices[i].ExtraCategory = extraCategory[i];
                processedProductPrices[i].Category1MinPrice = category1MinPrice[i];
                processedProductPrices[i].Category1MaxPrice = category1MaxPrice[i];
                processedProductPrices[i].Category2MinPrice = category2MinPrice[i];
                processedProductPrices[i].Category2MaxPrice = category2MaxPrice[i];
                processedProductPrices[i].QuantityToSupply = quantityToSupply[i];
                processedProductPrices[i].DominantPrice = dominantPrice[i];
                processedProductPrices[i].PreviousWeekDominantPrice = previousWeekDominantPrice[i];
                processedProductPrices[i].PreviousYearDominantPrice = previousYearDominantPrice[i];
                processedProductPrices[i].PreviousWeekPriceDifference = previousWeekPriceDifference[i];
                processedProductPrices[i].PreviousYearPriceDifference = previousYearPriceDifference[i];
                processedProductPrices[i].SoldQuantityPercentage = soldQuantityPercentage[i];
            }

            return processedProductPrices;
        }

        private ProductPrice ProductPriceExists(IEnumerable<ProductPrice> productPrices, DateTime date)
        {
            foreach (ProductPrice productPrice in productPrices)
            {
                if (productPrice.ProductDate == date.Ticks)
                {
                    return productPrice;
                }
            }
            return null;
        }

        private Dictionary<int, List<DateTime>> SplitDaysByYear(DateTime[] days)
        {
            Dictionary<int, List<DateTime>> daysByYear = new Dictionary<int, List<DateTime>>();

            foreach (DateTime day in days)
            {
                int year = day.Year;
                if (daysByYear.ContainsKey(year))
                {
                    daysByYear[year].Add(day);
                }
                else
                {
                    List<DateTime> daysList = new List<DateTime>();
                    daysList.Add(day);
                    daysByYear.Add(year, daysList);
                }
            }

            return daysByYear;
        }
    }
}

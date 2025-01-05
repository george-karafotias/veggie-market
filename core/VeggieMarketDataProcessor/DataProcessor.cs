using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataProcessor
{
    public class DataProcessor
    {
        public IEnumerable<ProductPrice> ProcessProductPrices(IEnumerable<ProductPrice> productPrices, int year)
        {
            DateTime begin = new DateTime(year, 1, 1);
            DateTime end = new DateTime(year, 12, 31);
            return ProcessAllProductPricesForPeriod(productPrices, begin, end);
        }

        public IEnumerable<ProductPrice> ProcessAllProductPricesForPeriod(IEnumerable<ProductPrice> productPrices, DateTime begin, DateTime end)
        {
            List<ProductPrice> processedProductPrices = new List<ProductPrice>();
            Product product = productPrices.First().Product;
            Market market = productPrices.First().Market;

            for (DateTime date = begin; date <= end; date = date.AddDays(1))
            {
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
    }
}

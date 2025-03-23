using System;
using System.Collections.Generic;
using VeggieMarketDataStore.Models;

namespace VeggieDataExporter
{
    public static class ProductPriceGrouping
    {
        public static Dictionary<int, List<ProductPrice>> GroupByYear(IEnumerable<ProductPrice> productPrices)
        {
            Dictionary<int, List<ProductPrice>> productPricesPerYear = new Dictionary<int, List<ProductPrice>>();

            foreach (ProductPrice productPrice in productPrices)
            {
                DateTime day = new DateTime(productPrice.ProductDate);
                int year = day.Year;
                if (!productPricesPerYear.ContainsKey(year))
                {
                    productPricesPerYear[year] = new List<ProductPrice>();
                }
                productPricesPerYear[year].Add(productPrice);
            }

            return productPricesPerYear;
        }
    }
}

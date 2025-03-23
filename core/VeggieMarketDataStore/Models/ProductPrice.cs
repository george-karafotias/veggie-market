using System;
using System.Collections.Generic;
using System.Reflection;

namespace VeggieMarketDataStore.Models
{
    public class ProductPrice
    {
        public Product Product { get; set; }
        public long ProductDate
        {
            get
            {
                return productDate;
            }
            set
            {
                productDate = value;
                DateTime dt = new DateTime(productDate);
                FormattedProductDate = dt.ToString("dd/MM/yyyy");
            }
        }
        public string FormattedProductDate { get; set; }
        public Market Market { get; set; }
        public double? ExtraCategory { get; set; }
        public double? Category1MinPrice { get; set; }
        public double? Category1MaxPrice { get; set; }
        public double? Category2MinPrice { get; set; }
        public double? Category2MaxPrice { get; set; }
        public double? QuantityToSupply { get; set; }
        public double? DominantPrice { get; set; }
        public double? PreviousWeekDominantPrice { get; set; }
        public double? PreviousYearDominantPrice { get; set; }
        public double? PreviousWeekPriceDifference { get; set; }
        public double? PreviousYearPriceDifference { get; set; }
        public double? SoldQuantityPercentage { get; set; }

        private long productDate;

        private static Dictionary<string, string> priceTypes = new Dictionary<string, string>()
        {
            { "Category 1 Min Price" , "Category1MinPrice" },
            { "Category 1 Max Price" , "Category1MaxPrice" },
            { "Category 2 Min Price" , "Category2MinPrice" },
            { "Category 2 Max Price" , "Category2MaxPrice" },
            { "Dominant Price" , "DominantPrice" },
            { "Previous Week Dominant Price" , "PreviousWeekDominantPrice" },
            { "Previous Year Dominant Price" , "PreviousYearDominantPrice" },
            { "Extra Category" , "ExtraCategory" },
            { "Quantity to Supply" , "QuantityToSupply" },
            { "Previous Week Price Difference" , "PreviousWeekPriceDifference" },
            { "Previous Year Price Difference" , "PreviousYearPriceDifference" },
            { "Sold Quantity Percentage" , "SoldQuantityPercentage" }
        };

        public ProductPrice(Product product, long productDate, Market market)
        {
            this.Product = product;
            this.ProductDate = productDate;
            this.Market = market;
        }

        public static Dictionary<string, string> GetPriceTypes()
        {
            return priceTypes;
        }

        public double? GetPriceType(string priceType)
        {
            PropertyInfo property = this.GetType().GetProperty(priceType);
            if (property != null)
            {
                var value = property.GetValue(this);
                return (double?)value;
            }
            return null;
        }
    }
}

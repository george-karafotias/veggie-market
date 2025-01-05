using System;

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

        public ProductPrice(Product product, long productDate, Market market)
        {
            this.Product = product;
            this.ProductDate = productDate;
            this.Market = market;
        }
    }
}

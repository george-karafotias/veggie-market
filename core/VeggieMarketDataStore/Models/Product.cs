using System.Collections.Generic;

namespace VeggieMarketDataStore.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public ProductType ProductType { get; set; }

        public List<ProductPrice> ProductPrices {get; set;}

        public Product() { }

        public Product(string productName, ProductType productType)
        {
            ProductName = productName;
            ProductType = productType;
        }

        public Product(int productId, string productName, ProductType productType) : this(productName, productType)
        {
            ProductId = productId;
        }
    }
}

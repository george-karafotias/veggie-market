namespace VeggieMarketDataStore.Models
{
    public class ProductPriceType
    {
        public string ProductPriceTypeId { get; set; }
        public string ProductPriceTypeName { get; set; }

        public ProductPriceType(string productPriceTypeId, string productPriceTypeName)
        {
            ProductPriceTypeId = productPriceTypeId;
            ProductPriceTypeName = productPriceTypeName;
        }
    }
}

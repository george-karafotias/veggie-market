namespace VeggieMarketDataStore.Models
{
    public class ProductType
    {
        public int ProductTypeId { get; set; }

        public string ProductTypeName { get; set; }

        public ProductType() { }

        public ProductType(string productTypeName)
        {
            ProductTypeName = productTypeName;
        }

        public ProductType(int productTypeId, string productTypeName) : this(productTypeName)
        {
            ProductTypeId = productTypeId;
        }
    }
}

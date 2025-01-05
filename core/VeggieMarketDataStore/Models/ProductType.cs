using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

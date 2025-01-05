using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore
{
    interface IDataStorageService
    {
        bool InsertProductType(ProductType productType);
        bool InsertProduct(Product product);
        bool InsertProductPrice(ProductPrice productPrice);
        ProductType GetProductType(string productTypeName);
        Product GetProduct(string productName);
        bool ProductHasPrice(int productId, long date);
    }
}

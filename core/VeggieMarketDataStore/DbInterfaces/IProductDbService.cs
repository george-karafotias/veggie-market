using System.Collections.Generic;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore.DbInterfaces
{
    public interface IProductDbService
    {
        bool InsertProduct(Product product);

        IEnumerable<Product> GetProducts();

        Product GetProduct(int productId);

        Product GetProduct(string productName);
    }
}

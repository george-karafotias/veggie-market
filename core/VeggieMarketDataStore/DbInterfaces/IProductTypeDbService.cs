using System.Collections.Generic;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore
{
    public interface IProductTypeDbService
    {
        bool InsertProductType(ProductType productType);

        IEnumerable<ProductType> GetProductTypes();

        ProductType GetProductType(string productTypeName, bool partialMatch = false);
    }
}
